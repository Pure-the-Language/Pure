using Console = Colorful.Console;

namespace Core
{
    public class Interpreter
    {
        #region Property
        private RoslynContext Context { get; set; }
        #endregion

        #region Methods
        public void Start(string welcomeMessage = null, bool advancedInterpretingMode = false, bool defaultPackages = true, string[] startingScripts = null, bool skipInteractiveMode = false, string[] arguments = null)
        {
            if (!string.IsNullOrWhiteSpace(welcomeMessage))
                Console.WriteLine(welcomeMessage);

            Context = new RoslynContext(true, null);
            if (arguments != null && arguments.Length != 0)
            {
                Context.Evaluate($"""
                    string[] Arguments = new string[{arguments.Length}];
                    """);
                for (int i = 0; i < arguments.Length; i++)
                {
                    string argument = arguments[i];
                    Context.Evaluate($"""
                        Arguments[{i}] = "{argument.Replace("\"", "\\\"")}";
                        """);
                }
            }
            else
                Context.Evaluate($"""
                    string[] Arguments = Array.Empty<string>();
                    """);
            if (startingScripts != null)
                foreach (var script in startingScripts)
                    Context.Evaluate(script);

            if (skipInteractiveMode)
                return;
            while (true)
            {
                Console.Write(">>> ");
                string input = Console.ReadLine().Trim();

                if (input == "exit" || input == "exit()")
                    return;
                Context.Evaluate(input);
            }
        }
        public void Start(Action<string> outputHandler, string welcomeMessage = null, string[] startingScripts = null)
        {
            Context = new RoslynContext(true, outputHandler);
            if (!string.IsNullOrWhiteSpace(welcomeMessage))
                Console.WriteLine(welcomeMessage);
            if (startingScripts != null)
                foreach (var script in startingScripts)
                    Context.Evaluate(script);
        }
        public void Evaluate(string script)
        {
            Context.Evaluate(script);
        }
        #endregion
    }
}