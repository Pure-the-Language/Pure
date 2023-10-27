using Core;

namespace CentralSnippetsTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var interpreter = new Interpreter(null, null, null, null, null);
            interpreter.Start();

            CentralSnippets.Main.Preview("Demos/HelloWorld.cs");
            CentralSnippets.Main.Download("Demos/HelloWorld.cs", "DownloadedHelloWorld.cs");
        }
    }
}