using Console = Colorful.Console;

namespace Core
{
    public class Interpreter
    {
        #region States
        private RoslynContext Context { get; set; }
        #endregion

        #region Properties
        public string WelcomeMessage { get; private set; }
        public string ScriptFile { get; private set; }
        public string[] Arguments { get; private set; }
        public string[] StartingScripts { get; private set; }
        /// <summary>
        /// Configures the identification for current running instance for the purpose of caching and compiling nuget package references
        /// </summary>
        public string NugetRepoIdentifier { get; private set; }
        #endregion

        #region Constructor
        public Interpreter(string welcomeMessage, string scriptFile, string[] arguments, string[] startingScripts, string nugetRepoIdentifier)
        {
            WelcomeMessage = welcomeMessage;
            ScriptFile = scriptFile;
            Arguments = arguments;
            StartingScripts = startingScripts;
            NugetRepoIdentifier = nugetRepoIdentifier;
        }
        #endregion

        #region Methods
        public void Start(bool skipInteractiveMode = false)
        {
            Context = new RoslynContext(true, null, NugetRepoIdentifier);
            if (!string.IsNullOrWhiteSpace(WelcomeMessage))
                Console.WriteLine(WelcomeMessage);

            UpdateScriptArguments(Arguments);
            if (StartingScripts != null)
                foreach (var script in StartingScripts)
                    Context.Evaluate(script, ScriptFile, NugetRepoIdentifier);

            if (skipInteractiveMode)
                return;
            while (true)
            {
                Console.Write(">>> ");
                string input = Console.ReadLine().Trim();

                if (input == "exit" || input == "exit()")
                    return;
                Context.Evaluate(input, ScriptFile, NugetRepoIdentifier);
            }
        }
        public void Start(Action<string> outputHandler)
        {
            Context = new RoslynContext(true, outputHandler, NugetRepoIdentifier);
            if (!string.IsNullOrWhiteSpace(WelcomeMessage))
                Console.WriteLine(WelcomeMessage);

            UpdateScriptArguments(Arguments);
            if (StartingScripts != null)
                foreach (var script in StartingScripts)
                    Context.Evaluate(script, ScriptFile, NugetRepoIdentifier);
        }
        public void Evaluate(string script)
            => Context.Evaluate(script, ScriptFile, NugetRepoIdentifier);
        #endregion

        #region Routines
        public void UpdateScriptFilePath(string scriptFile)
        {
            ScriptFile = scriptFile;
        }
        public void UpdateNugetRepositoryIdentifier(string nugetRepoIdentifier)
        {
            NugetRepoIdentifier = nugetRepoIdentifier;
        }
        public void UpdateScriptArguments(string[] arguments)
        {
            Arguments = arguments;
            if (arguments != null && arguments.Length != 0)
            {
                Context.Evaluate($"""
                    string[] Arguments = new string[{arguments.Length}];
                    """, ScriptFile, NugetRepoIdentifier);
                for (int i = 0; i < arguments.Length; i++)
                {
                    string argument = arguments[i];
                    Context.Evaluate($"""
                        Arguments[{i}] = @"{argument.Replace("\"", "\\\"")}";
                        """, ScriptFile, NugetRepoIdentifier);
                }
            }
            else
                Context.Evaluate($"""
                    string[] Arguments = Array.Empty<string>();
                    """, ScriptFile, NugetRepoIdentifier);
        }
        #endregion
    }
}