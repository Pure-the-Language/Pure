using System.Diagnostics;

namespace Core.Utilities
{
    public class Measure : IDisposable
    {
        Stopwatch Timer;
        public Measure()
        {
            Timer = new Stopwatch();
            Timer.Start();
        }
        public void Dispose()
        {
            Timer.Stop();
            TimeSpan timeTaken = Timer.Elapsed;
            Console.WriteLine("Time taken: " + timeTaken.ToString(@"m\:ss\.fff"));
        }
    }
}
