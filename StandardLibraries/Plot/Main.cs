using Core.Helpers;
using ScottPlot;
using System.Diagnostics;
using System.Reflection;

namespace Graphing
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
            => GeneralParsingRoutine(PlotType.Line, x, ys, settings);
        /// <summary>
        /// Draw or render a line chart
        /// </summary>
        public static void LineChart(double[] x, double[] y, params string[] settings)
            => GeneralParsingRoutine(PlotType.Line, x, new List<double[]> { y }, settings);
        /// <summary>
        /// Plot histogram into given number of bars.
        /// </summary>
        public static void Histogram(double[] v, int slots, params string[] additionalSettings)
            => GeneralParsingRoutine(PlotType.Histogram, null, new List<double[]> { v }, additionalSettings.Concat(new string[] { $"--{nameof(PlotOptions.HistogramBars)}", slots.ToString() }).ToArray());
        /// <summary>
        /// Draw a signal chart
        /// </summary>
        public static void Signal(double[] v, int sampleRate, params string[] additionalSettings)
            => GeneralParsingRoutine(PlotType.Signal, null, new List<double[]> { v }, additionalSettings.Concat(new string[] { $"--{nameof(PlotOptions.SignalSampleRate)}", sampleRate.ToString() }).ToArray());
        /// <summary>
        /// Draw a signal chart
        /// </summary>
        public static void Signal(List<double[]> vs, int sampleRate, params string[] additionalSettings)
            => GeneralParsingRoutine(PlotType.Signal, null, vs, additionalSettings.Concat(new string[] { $"--{nameof(PlotOptions.SignalSampleRate)}", sampleRate.ToString() }).ToArray());

        #region Routines
        private static void GeneralParsingRoutine(PlotType plotType, double[] x, List<double[]> ys, params string[] settings)
        {
            PlotOptions options = CookOptions(settings);
            Main.Execute(plotType, x, ys, options);
        }
        /// <summary>
        /// Make options from string arguments.
        /// </summary>
        public static PlotOptions CookOptions(string[] settings)
        {
            // Prepare arguments
            if (settings.Length == 0 || !settings.Any(s => s.EndsWith(".png")))
                settings = new string[] { $"--{nameof(PlotOptions.Interactive)}" };
            else if (!settings.First().StartsWith("--"))
                settings = new string[] { $"--{nameof(PlotOptions.OutputImage)}" }.Concat(settings).ToArray();

            // Parse arguments
            var options = CLI.Main.Parse<PlotOptions>(settings);
            return options;
        }
        /// <summary>
        /// Initialize plot based on type and data
        /// </summary>
        public static Plot InitializePlot(PlotType plotType, double[] x, List<double[]> ys, PlotOptions options)
        {
            Plot plot = new();

            switch (plotType)
            {
                case PlotType.Scatter:
                    for (int i = 0; i < ys.Count; i++)
                    {
                        double[] y = ys[i];
                        if (i < options.Labels.Length)
                        {
                            var s = plot.Add.Scatter(x, y);
                            s.Label = options.Labels[i];
                        }
                        else
                            plot.Add.Scatter(x, y);
                    }
                    break;
                case PlotType.Line:
                    for (int i = 0; i < ys.Count; i++)
                    {
                        double[] y = ys[i];
                        if (i < options.Labels.Length)
                        {
                            var s = plot.Add.Scatter(x, y);
                            s.Label = options.Labels[i];
                        }
                        else
                            plot.Add.Scatter(x, y);
                    }
                    break;
                case PlotType.Signal:
                    for (int i = 0; i < ys.Count; i++)
                    {
                        double[] y = ys[i];
                        if (i < options.Labels.Length)
                        {
                            var s = plot.Add.Signal(y, options.SignalSampleRate);
                            s.Label = options.Labels[i];
                        }
                        else
                            plot.Add.Signal(y, options.SignalSampleRate);
                    }
                    break;
                case PlotType.Histogram:
                    double[] v = ys.Single();
                    ScottPlot.Statistics.Histogram hist = new(min: v.Min(), max: v.Max(), binCount: options.HistogramBars);
                    hist.AddRange(v);
                    plot.Add.Bars(values: hist.Counts, positions: hist.Bins);
                    break;
                default:
                    break;
            }

            // Enable additional drawing
            if (options.DrawTitle)
                plot.Title(options.Title);
            if (options.DrawAxes)
            {
                plot.Axes.Left.Label.Text = options.XAxis;
                plot.Axes.Bottom.Label.Text = options.YAxis;
            }
            if (options.Labels.Length > 0)
                plot.Legend.IsVisible = true;

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
            string defaultPath = Path.Combine(GetAssemblyFolder(), "Windows", executableName);
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

        #region Utilities
        /// <summary>
        /// Make a list.
        /// </summary>
        public static List<double[]> Make(params double[][] vs)
            => vs.ToList();
        /// <summary>
        /// Make options.
        /// </summary>
        public static PlotOptions Make(string arguments)
            => Plotters.CookOptions(arguments.Split(' ', StringSplitOptions.RemoveEmptyEntries));
        #endregion

        #region Plot
        /// <summary>
        /// Plot interaactively
        /// </summary>
        public static void Plot(double[] x, params double[][] ys)
            => Plot(PlotType.Line, x, ys.ToList());
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
                plt.SavePng(options.OutputImage, options.WindowWidth, options.WindowHeight);
            }
            if (options.Interactive)
                Plotters.SummonInteractiveWindow(plotType, x, ys, options);
        }
        #endregion
    }
}