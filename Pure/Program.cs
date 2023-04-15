using Core;

namespace Pure
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
                new Interpreter().Start($"""
                    Pure v0.0.3 (Core: {Core.Parser.CoreVersion})
                    """);
            else if (args.Length >= 2 && (args[0] == "-m" || args[0] == "-mi"))
            {
                new Interpreter().Start(string.Empty, false, true, new string[] { args[1] }, args[0] == "-m", args.Skip(2).ToArray());
            }
            else if (args.Length >= 1) 
            {
                bool interactiveMode = args.Length >= 2 && (args[0].ToLower() == "-i" || args[0].ToLower() == "--interactive");
                string fileName = interactiveMode ? args[1] : args[0];

                string file = Path.GetFullPath(fileName);
                if (!File.Exists(file))
                {
                    Console.WriteLine($"File {file} doesn't exist.");
                    return; 
                }
                new Interpreter().Start(string.Empty, false, true, Parser.SplitScripts(File.ReadAllText(file)), !interactiveMode, interactiveMode ? args.Skip(2).ToArray() : args.Skip(1).ToArray());
            }
        }
    }
}