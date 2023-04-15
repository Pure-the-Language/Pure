using Console = Colorful.Console;

namespace Core
{
    public class Interpreter
    {
        #region Property
        private RoslynContext Context { get; set; }
        public string[] Arguments { get; private set; }
        #endregion

        #region Methods
        public void Start(string welcomeMessage = null, bool advancedInterpretingMode = false, bool defaultPackages = true, string[] startingScripts = null, bool skipInteractiveMode = false, string[] arguments = null, string nugetRepoIdentifier = null)
        {
            Context = new RoslynContext(true, null, nugetRepoIdentifier);
            if (!string.IsNullOrWhiteSpace(welcomeMessage))
                Console.WriteLine(welcomeMessage);

            InitializeArguments(arguments);
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
        public void Start(Action<string> outputHandler, string welcomeMessage = null, string[] startingScripts = null, string[] arguments = null, string nugetRepoIdentifier = null)
        {
            Context = new RoslynContext(true, outputHandler, nugetRepoIdentifier);
            if (!string.IsNullOrWhiteSpace(welcomeMessage))
                Console.WriteLine(welcomeMessage);

            InitializeArguments(arguments);
            if (startingScripts != null)
                foreach (var script in startingScripts)
                    Context.Evaluate(script);
        }
        public void Evaluate(string script)
        {
            Context.Evaluate(script);
        }
        #endregion

        #region Routines
        public void SetNugetRepositoryIdentifier(string nugetRepoIdentifier)
        {
            Context.NugetRepoIdentifier = nugetRepoIdentifier;
        }
        public void InitializeArguments(string[] arguments)
        {
            Arguments = arguments;
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
        }
        #endregion
    }
}