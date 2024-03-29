﻿using Core;
using Core.Helpers;

namespace Pure
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Help
            if (args.Length == 1 && args.Single().ToLower() == "--help")
            {
                Console.WriteLine($"""
                    Interactive Pure Interpreter (REPL) (Core Version: {Interpreter.CoreVersion})
                    CLI Variants:
                      pure: REPL mode
                      pure --version: Print version
                      pure --help: Print this help
                      pure <File Path> [<Arguments>...]: Run script from default paths (current folder, PUREPATH)
                      pure -i <File Path> [<Arguments>...]: Run script from default paths (current folder, PUREPATH) and enter interactive mode
                      pure -m <Expression> [<Arguments>...]: Evaluate expression
                      pure -l/mi <File Path> <Expression>: Load file then evaluate expression
                    """);
            }
            // Version
            else if (args.Length == 1 && args.Single().ToLower() == "--version")
                Console.WriteLine(Interpreter.CoreVersion);
            // REPL mode
            else if (args.Length == 0)
            {
                var interpreter = new Interpreter($"""
                    Interactive Pure Interpreter (REPL) (Core Version: {Interpreter.CoreVersion})
                    This is the frontend to Pure, the scripting language.
                    """, null, null, null, null);
                interpreter.Start();
                EnterInteractiveMode(interpreter);
            }
            // Evaluate expression
            else if (args.Length >= 2 && args[0] == "-m")
                new Interpreter(string.Empty, null, args.Skip(2).ToArray(), new string[] { args[1] }, null)
                    .Start();
            // Load file and evaluate
            else if (args.Length == 3 && (args[0] == "-l" || args[0] == "-mi"))
            {
                string fileName = args[1];
                string expression = args[2];
                string scriptPath = PathHelper.FindScriptFileFromEnvPath(fileName);
                new Interpreter(string.Empty, scriptPath, null, Interpreter.SplitScripts(File.ReadAllText(scriptPath)).Concat(new string[] { expression }).ToArray(), null)
                    .Start();
            }
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
                interpreter.Parse(input);
            }
        }
    }
}