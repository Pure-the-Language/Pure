using Core;
using System.Diagnostics;
using System.Reflection;

namespace Plot
{
    public static class Main
    {
        public static void Scatter(double[] x, double[] y, params string[] settings)
        {
            var plt = new ScottPlot.Plot(400, 300);
            plt.AddScatter(x, y);
            if (settings.Length == 0)
                SummonInteractiveWindow(PlotType.Scatter, x, new List<double[]> { y });
            foreach (var setting in settings)
            {
                if (setting.EndsWith(".png"))
                    plt.SaveFig(setting);
                switch (setting)
                {
                    case "--interactive":
                        SummonInteractiveWindow(PlotType.Scatter, x, new List<double[]> { y });
                        break;
                    default:
                        break;
                }
            }
        }

        public static void SummonInteractiveWindow(PlotType plotType, double[] x, IList<double[]> ys)
        {
            string filePath = Path.GetTempFileName();
            InteractivePlotData.SaveData(new InteractivePlotData()
            {
                PlotType = plotType,
                X = x,
                Ys = ys.ToList()
            }, filePath);
            string programPath = Path.Combine(GetAssemblyFolder(), "PlotWindow.exe");
            if (!File.Exists(programPath))
                programPath = RoslynContext.TryFindDLLFile(Path.Combine("Plot", "PlotWindow.exe"));
            if (!File.Exists(programPath))
            {
                Console.WriteLine($"Failed to find executable path: {programPath}");
                return; // Remark: Fail silently
            }
            Process.Start(new ProcessStartInfo(programPath, $"\"{filePath}\"")
            {

            }).WaitForExit();
        }

        #region Helper
        public static string GetAssemblyFolder()
        {
            string codeBase = Assembly.GetExecutingAssembly().Location;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }
        #endregion
    }
}