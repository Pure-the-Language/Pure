namespace PlotTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var x = Enumerable.Range(0, 100).Select(v => (double)v).ToArray();
            var y = x.Select(v => v*2).ToArray();
            var z = x.Select(v => v*v).ToArray();

            Plot.Plotters.Scatter(x, new List<double[]> { x, y, z}, "--Labels", "X", "Y", "Z");
            Plot.Plotters.Signal(x, 5, "Output_Signal.png");
        }
    }
}