using Graphing;
using ScottPlot;
using System.IO;

namespace PlotWindow
{
    internal class Program
    {
        #region Entrance
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("""
                    Usage:
                      PlotWindow <Transient Data Transfer>: Used as backend for Plot; Render per instruction from data transfer file.
                      PlotWindow <Input CSV> <Formatting>: Draw an interactive window from data read from CSV file.
                    """);
            }
            else if (args.Length > 1 || args.First().EndsWith(".csv"))
                PlotAsCLI(args);
            else if (args.Length == 1)
                PlotAsBackend(args.Single());
        }
        #endregion

        #region Modes
        private static void PlotAsBackend(string transientDataFile)
        {
            // Read transient data
            string configurationsFile = transientDataFile;
            InteractivePlotData data = InteractivePlotData.LoadData(configurationsFile);

            // Initialize plot
            Plot plt = Plotters.InitializePlot(data.PlotType, data.X, data.Ys, data.Options);

            // Show
            ShowPlot(plt);
        }
        private static void PlotAsCLI(string[] args)
        {
            // Read CSV
            string csvFile = args[0];
            string[] configurations = args.Skip(1).ToArray();
            (double[] X, List<double[]> Ys) values = GetData(csvFile);
            InteractivePlotData data = new()
            {
                X = values.X,
                Ys = values.Ys,
                PlotType = PlotType.Scatter,
                Options = GetPlotOptions(configurations)
            };

            // Initialize plot
            Plot plot = Plotters.InitializePlot(data.PlotType, data.X, data.Ys, data.Options);

            // Show
            ShowPlot(plot);
        }
        #endregion

        #region Routines
        private static (double[] X, List<double[]> Ys) GetData(string csvFile)
        {
            List<double> x = new();
            List<List<double>> ys = new();
            foreach (var line in File.ReadLines(csvFile))
            {
                string[] parts = line.Split(',');
                try
                {
                    double[] values = parts.Select(double.Parse).ToArray();
                    x.Add(values.First());
                    foreach (var item in values.Skip(1).Skip(ys.Count))
                        ys.Add(new());
                    for (int i = 1; i < values.Length; i++)
                        ys[i - 1].Add(values[i]);
                }
                catch (Exception)
                {
                    // Remark-cz: Silently skip header lines and invalid lines.
                    Console.WriteLine($"Skip line: {line}");
                    continue;
                }
            }
            return (x.ToArray(), ys.Select(y => y.ToArray()).ToList());
        }
        private static PlotOptions GetPlotOptions(string[] configurations)
        {
            var options = CLI.Main.Parse<PlotOptions>(configurations);
            if (configurations.Length == 0)
                options.Interactive = true;
            return options;
        }
        #endregion

        #region Helpers
        private static void ShowPlot(Plot plot)
        {
            ScottPlot.WPF.WpfPlot wpfPlot = new();
            wpfPlot.Reset(plot);
            System.Windows.Window win = new() { Content = wpfPlot };
            win.ShowDialog();
        }
        #endregion
    }
}