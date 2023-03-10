namespace Plot
{
    public static class Main
    {
        public static void Scatter(double[] x, double[] y, params string[] settings)
        {
            var plt = new ScottPlot.Plot(400, 300);
            plt.AddScatter(x, y);
            if (settings.Length == 0)
                new ScottPlot.WpfPlotViewer(plt).ShowDialog();
            foreach (var setting in settings)
            {
                if (setting.EndsWith(".png"))
                    plt.SaveFig(setting);
                switch (setting)
                {
                    case "--interactive":
                        new ScottPlot.WpfPlotViewer(plt).ShowDialog();
                        break;
                    default:
                        break;
                }
            }
        }
    }
}