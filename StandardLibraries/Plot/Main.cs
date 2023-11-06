using Core.Helpers;
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
        /// Draw or render a scatter plot; Currently looks the same as line chart
        /// </summary>
        public static void Scatter(double[] x, double[] y, params string[] settings)
            => GeneralParsingRoutine(PlotType.Scatter, x, new List<double[]> { y }, settings);
        /// <summary>
        /// Draw or render a scatter plot; Currently looks the same as line chart
        /// </summary>
        public static void Scatter(double[] x, List<double[]> ys, params string[] settings)
            => GeneralParsingRoutine(PlotType.Scatter, x, ys, settings);
        /// <summary>
        /// Draw or render a line chart
        /// </summary>
        public static void LineChart(double[] x, List<double[]> ys, params string[] settings)
            => GeneralParsingRoutine(PlotType.LineChart, x, ys, settings);
        /// <summary>
        /// Draw or render a line chart
        /// </summary>
        public static void LineChart(double[] x, double[] y, params string[] settings)
            => GeneralParsingRoutine(PlotType.LineChart, x, new List<double[]> { y }, settings);
        /// <summary>
        /// Draw a signal chart
        /// </summary>
        public static void Signal(double[] v, int sampleRate, params string[] additionalSettings)
            => GeneralParsingRoutine(PlotType.Signal, null, new List<double[]> { v }, additionalSettings.Concat(new string[] { $"--{nameof(PlotOptions.SignalSampleRate)}", sampleRate.ToString(), }).ToArray());
        /// <summary>
        /// Draw a signal chart
        /// </summary>
        public static void Signal(List<double[]> vs, int sampleRate, params string[] additionalSettings)
            => GeneralParsingRoutine(PlotType.Signal, null, vs, additionalSettings.Concat(new string[] { $"--{nameof(PlotOptions.SignalSampleRate)}", sampleRate.ToString(), }).ToArray());

        #region Routines
        private static void GeneralParsingRoutine(PlotType plotType, double[] x, List<double[]> ys, params string[] settings)
        {
            // Parse arguments
            if (settings.Length == 0)
                settings = new string[] { $"--{nameof(PlotOptions.Interactive)}" };
            else if (!settings.First().StartsWith("--"))
                settings = new string[] { $"--{nameof(PlotOptions.OutputImage)}" }.Concat(settings).ToArray();
            var options = CLI.Main.Parse<PlotOptions>(settings);
            Main.Execute(plotType, x, ys, options);
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
                case PlotType.Signal:
                    foreach (var y in ys)
                        plot.AddSignal(y, options.SignalSampleRate);
                    break;
                default:
                    break;
            }
            return plot;
        }
        #endregion

        #region Interactivity
        /// <summary>
        /// Create display using interactive window
        /// </summary>
        public static void SummonInteractiveWindow(PlotType plotType, double[] x, List<double[]> ys, PlotOptions options)
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
                Ys = ys,
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
        #region Defaults
        /// <summary>
        /// Default plot options
        /// </summary>
        public static PlotOptions DefaultOptions => new();
        #endregion

        #region Plot
        /// <summary>
        /// Plot interaactively
        /// </summary>
        public static void Plot(string plotType, double[] x, List<double[]> ys, PlotOptions options = null)
            => Plot(Enum.Parse<PlotType>(plotType), x, ys, options);
        /// <summary>
        /// Plot interaactively
        /// </summary>
        public static void Plot(PlotType plotType, double[] x, List<double[]> ys, PlotOptions options = null)
        {
            options ??= DefaultOptions;
            options.Interactive = true;

            Execute(plotType, x, ys, options);
        }
        #endregion

        #region Save
        /// <summary>
        /// Save to image
        /// </summary>
        public static void Save(string plotType, double[] x, List<double[]> ys, string output, PlotOptions options = null)
            => Save(Enum.Parse<PlotType>(plotType), x, ys, output, options);
        /// <summary>
        /// Save to image
        /// </summary>
        public static void Save(PlotType plotType, double[] x, List<double[]> ys, string output, PlotOptions options = null)
        {
            options ??= DefaultOptions;
            options.OutputImage = output;

            Execute(plotType, x, ys, options);
        }
        #endregion

        #region Routine
        /// <summary>
        /// Execute graphing per options and plot type.
        /// </summary>
        public static void Execute(PlotType plotType, double[] x, List<double[]> ys, PlotOptions options)
        {
            // Basic inputs check
            if (plotType != PlotType.Signal
                && ys.Any(y => y.Length != x.Length))
                throw new ArgumentException($"Mismatch data point size {ys.First(y => y.Length != x.Length).Length} (from y) vs {x.Length} (from x).");

            // Generate output or display
            if (options.OutputImage != null && options.OutputImage.EndsWith(".png"))
            {
                ScottPlot.Plot plt = Plotters.InitializePlot(plotType, x, ys, options);
                plt.SaveFig(options.OutputImage);
            }
            if (options.Interactive)
                Plotters.SummonInteractiveWindow(plotType, x, ys, options);
        }
        #endregion
    }
}