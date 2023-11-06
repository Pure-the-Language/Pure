using Core.Helpers;
using Microsoft.CodeAnalysis;
using System.Diagnostics;
using System.Reflection;

namespace Plot
{
    /// <summary>
    /// Static class of various specific plotting types
    /// </summary>
    public static class Plotters
    {
        /// <summary>
        /// Default plot options
        /// </summary>
        public static PlotOptions DefaultOptions => new();

        /// <summary>
        /// Draw or render a scatter plot; Currently looks the same as line chart
        /// </summary>
        public static void Scatter(double[] x, double[] y, params string[] settings)
            => GeneralRoutine(PlotType.Scatter, x, new List<double[]> { y }, settings);
        /// <summary>
        /// Draw or render a line chart
        /// </summary>
        public static void LineChart(double[] x, double[] y, params string[] settings)
            => GeneralRoutine(PlotType.LineChart, x, new List<double[]> { y }, settings);

        #region Routines
        private static void GeneralRoutine(PlotType plotType, double[] x, List<double[]> ys, params string[] settings)
        {
            if (settings.Length == 0)
                settings = new string[] { "--interactive" };

            PlotOptions options = DefaultOptions;
            foreach (string setting in settings)
            {
                if (setting.EndsWith(".png"))
                {
                    options.SaveImage = true;
                    options.ImageOutput = setting;
                }
                switch (setting)
                {
                    case "--interactive":
                        SummonInteractiveWindow(plotType, x, ys, options);
                        break;
                    default:
                        break;
                }
            }

            ScottPlot.Plot plt = InitializePlot(plotType, x, ys, options);
            if (options.SaveImage)
                plt.SaveFig(options.ImageOutput);
        }

        /// <summary>
        /// Initialize plot based on type and data
        /// </summary>
        public static ScottPlot.Plot InitializePlot(PlotType plotType, double[] x, List<double[]> ys, PlotOptions options)
        {
            ScottPlot.Plot plot = new(options.WindowWidth, options.WindowHeight);

            switch (plotType)
            {
                case PlotType.Scatter:
                    foreach (var y in ys)
                        plot.AddScatter(x, y);
                    break;
                case PlotType.LineChart:
                    foreach (var y in ys)
                        plot.AddScatter(x, y);
                    break;
                default:
                    break;
            }
            return plot;
        }
        #endregion

        #region Interactivity
        public static void SummonInteractiveWindow(PlotType plotType, double[] x, IList<double[]> ys, PlotOptions options)
        {
            string executableName = "PlotWindow.exe";
            string defaultPath = Path.Combine(GetAssemblyFolder(), executableName);
            string secondaryPath = PathHelper.FindDLLFileFromEnvPath(executableName);

            // Find plot window executable
            string backendPath = defaultPath;
            if (!File.Exists(backendPath))
                backendPath = secondaryPath;
            if (!File.Exists(backendPath))
            {
                Console.WriteLine($"Failed to find executable from paths: neither {defaultPath} or {secondaryPath}");
                return; // Remark: Fail silently
            }

            // Create intermediate file
            string filePath = Path.GetTempFileName();
            InteractivePlotData.SaveData(new InteractivePlotData()
            {
                PlotType = plotType,
                X = x,
                Ys = ys.ToList(),
                Options = options
            }, filePath);

            // Summon process
            Process.Start(new ProcessStartInfo(backendPath, $"\"{filePath}\""));
        }
        #endregion

        #region Helper
        /// <summary>
        /// Get folder path of currently executing assembly
        /// </summary>
        public static string GetAssemblyFolder()
        {
            string codeBase = Assembly.GetExecutingAssembly().Location;
            UriBuilder uri = new(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }
        #endregion
    }

    /// <summary>
    /// Standard library entry
    /// </summary>
    public static class Main
    {

    }
}