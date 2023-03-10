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

            InteractivePlotData data = InteractivePlotData.LoadData(args[0]);
            var plt = new ScottPlot.Plot(400, 300);
            foreach (double[] item in data.Ys)
                plt.AddScatter(data.X, item);
            new ScottPlot.WpfPlotViewer(plt).ShowDialog();
        }
    }
}