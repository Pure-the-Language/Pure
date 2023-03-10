using Core;

namespace Pure
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
                new Interpreter().Start("""
                    Pure v0.0.1
                    """);
            else if (args.Length >= 1) 
            {
                bool interactiveMode = args.Length == 2 && (bool.Parse(args[0]) || args[0].ToLower() == "-i" || args[0].ToLower() == "--interactive");
                string fileName = args.Length == 2 ? args[1] : args[0];

                string file = Path.GetFullPath(fileName);
                if (!File.Exists(file))
                {
                    Console.WriteLine($"File {file} doesn't exist.");
                    return; 
                }
                new Interpreter().Start(string.Empty, false, true, Parser.SplitScripts(File.ReadAllText(file)), !interactiveMode);
            }
        }
    }
}