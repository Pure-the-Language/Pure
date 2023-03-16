using System.Reflection;

namespace PythonTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Startup...");
            typeof(Python.Main).GetMethod("StartUp", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, null);
            Console.WriteLine("Shutdown...");
            typeof(Python.Main).GetMethod("ShutDown", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, null);
        }
    }
}