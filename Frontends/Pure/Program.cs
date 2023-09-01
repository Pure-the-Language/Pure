using Core;
using Core.Helpers;

namespace Pure
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // REPL mode
            if (args.Length == 0)
            {
                var interpreter = new Interpreter($"""
                    Interactive Pure Interpreter (REPL) v0.0.4 (Core Version: {Interpreter.CoreVersion})
                    This is the frontend to Pure, the scripting language.
                    """, null, null, null, null);
                interpreter.Start();
                EnterInteractiveMode(interpreter);
            }
            // Immediate mode
            else if (args.Length >= 2 && (args[0] == "-m" || args[0] == "-mi"))
                new Interpreter(string.Empty, null, args.Skip(2).ToArray(), new string[] { args[1] }, null)
                    .Start();
            // Execute file with optional interactivity
            else if (args.Length >= 1) 
            {
                bool interactiveMode = args.Length >= 2 && (args[0].ToLower() == "-i" || args[0].ToLower() == "--interactive");
                string fileName = interactiveMode ? args[1] : args[0];

                string file = PathHelper.FindScriptFileFromEnvPath(fileName);
                if (!File.Exists(file))
                {
                    Console.WriteLine($"File {file} doesn't exist.");
                    return; 
                }
                var interpreter = new Interpreter(string.Empty, file, interactiveMode ? args.Skip(2).ToArray() : args.Skip(1).ToArray(), Interpreter.SplitScripts(File.ReadAllText(file)), file.GetDeterministicHashCode().ToString());
                interpreter.Start();
                if(interactiveMode)
                    EnterInteractiveMode(interpreter);
            }
        }

        private static void EnterInteractiveMode(Interpreter interpreter)
        {
            while (true)
            {
                Console.Write(">>> ");
                string input = Console.ReadLine().Trim();

                if (input == "exit" || input == "exit()")
                    return;
                interpreter.Evaluate(input);
            }
        }
    }
}