namespace Plot
{
    public static class Main
    {
        public static void Scatter(double[] x, double[] y, params string[] settings)
        {
            var plt = new ScottPlot.Plot(400, 300);
            plt.AddScatter(x, y);
            new ScottPlot.WpfPlotViewer(plt).ShowDialog();
            // plt.SaveFig("quickstart.png");
        }
    }
}