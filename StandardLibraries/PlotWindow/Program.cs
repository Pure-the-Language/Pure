using Plot;

namespace PlotWindow
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length != 1) 
            {
                Console.WriteLine("Not enough arguments.");
                return;
            }

            // Read transient data
            string configurationsFile = args[0];
            InteractivePlotData data = InteractivePlotData.LoadData(configurationsFile);

            // Initialize plot
            ScottPlot.Plot plt = Plotters.InitializePlot(data.PlotType, data.X, data.Ys, data.Options);

            // Show
            new ScottPlot.WpfPlotViewer(plt)
                .ShowDialog();
        }
    }
}