using Core;

namespace Pure
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
                new Interpreter($"""
                    Pure Interpreter v0.0.3 (Core: {Parser.CoreVersion})
                    """, null, null, null, null)
                    .Start();
            else if (args.Length >= 2 && (args[0] == "-m" || args[0] == "-mi"))
                new Interpreter(string.Empty, null, args.Skip(2).ToArray(), new string[] { args[1] }, null)
                    .Start(true);
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
                new Interpreter(string.Empty, file, interactiveMode ? args.Skip(2).ToArray() : args.Skip(1).ToArray(), Parser.SplitScripts(File.ReadAllText(file)), file.GetDeterministicHashCode().ToString()).Start(!interactiveMode);
            }
        }
    }
}