using System.Text.RegularExpressions;
using System.Text;
using Console = Colorful.Console;
using Core.Utilities;
using System.Reflection;
using Core.Services;

namespace Core
{
    /// <summary>
    /// Main provider for external frontends, this is the interface into RoslynContext.
    /// </summary>
    public class Interpreter
    {
        #region Versioning
        public static readonly string CoreVersion = "v0.7.0";
        public static readonly string VersionChangelog = """
            * v0.0.1-v0.0.3: Misc. basic functional implementations.
            * v0.0.3: Add functional Nuget implementation; Add support for `nugetRepoIdentifier:string` parameter for Interpreter and Roslyn Context.
            * v0.0.4: Change exception handling logic, print stack trace for easier debugging.
            * v0.0.5: Update runtime exception handling behavior.
            * v0.0.5.1: Fix issue with literal numerical array parsing.
            * v0.1.0: Fix semantics of `Include()`; Bump minor version number; Refactor `Interpreter` interface, merge Parser into Interpreter.
            * v0.1.1: Implement `Evaluate()` as part of Construct; Enhance `Help` outputs.
            * v0.1.2: Update handling of search paths for dll and scripts.
            * v0.2.0: Update definition of `Vector` type.
            ======Breaking Change======
            * v0.3.0: Rename Evaluate() to Parse(); Implement Interpreter.Evaluate() for expressions.
            ======Breaking Change======
            * v0.4.0: Remove `#` style line comment (potential conflict with #region due to how we parse it).
            * v0.5.0: Add add-on standard library CentralSnippets. Implement construct `Pull()` and `Preview()`.
            * v0.5.1: Enhance Roslyn Context with script record.
            * v0.5.2: Implement language level `Parse()` and `Pull()` handling.
            * v0.5.3: Simplify Help output for methods of specific type.
            ======Breaking Change======
            * v0.6.0: Remove syntactic wrap for vector/array creation. Move and enhance Vector type to standard library.
            * v0.6.1: Enhance output message for Importing module.
            * v0.6.2: Publish Core as NuGet.
            * v0.6.3: Allow adding additional assembly references upon interpreter initialization.
            * v0.6.4: Specify framework version when generating NuGet package compilation project during import.
            * v0.6.5: Fix issue whem importing libraries containing types in null namespace (e.g. Octokit).
            * v0.6.6: Rename Interpreter.ScriptFile to Interpreter.ScriptPath for semantic clarity.
            ======Runtime Change======
            * v0.7.0: Upgrade to .Net 8; Refactor code for adding default Libraries folder to PATH from RoslynContext to Interpreter.
            """;
        #endregion

        #region States
        private RoslynContext Context { get; set; }
        #endregion

        #region Properties
        public string WelcomeMessage { get; private set; }
        /// <summary>
        /// Path of script, used for resolving relative paths
        /// </summary>
        public string ScriptPath { get; private set; }
        public string[] Arguments { get; private set; }
        public string[] StartingScripts { get; private set; }
        /// <summary>
        /// Configures the identification for current running instance for the purpose of caching and compiling nuget package references
        /// </summary>
        public string NugetRepoIdentifier { get; private set; }
        #endregion

        #region Constructor
        public Interpreter(string welcomeMessage, string scriptPath, string[] arguments, string[] startingScripts, string nugetRepoIdentifier)
        {
            WelcomeMessage = welcomeMessage;
            ScriptPath = scriptPath;
            Arguments = arguments;
            StartingScripts = startingScripts;
            NugetRepoIdentifier = nugetRepoIdentifier;

            // Singleton-pattern global state
            Construct.CurrentInterpreter = this;
        }
        #endregion

        #region Methods
        public void Start(Action<string> outputHandler = null, IEnumerable<Assembly> additionalReferences = null, string[] additionalLibraryDllPaths = null)
        {
            // Pure core standard libraries
            AddDefaultLibraryFolderToEnvironmentPath();
            if (additionalLibraryDllPaths != null)
                foreach (var path in additionalLibraryDllPaths)
                    AddToEnvironmentPath(path);

            Context = new RoslynContext(true, outputHandler, additionalReferences);
            if (!string.IsNullOrWhiteSpace(WelcomeMessage))
                Console.WriteLine(WelcomeMessage);

            UpdateScriptArguments(Arguments);
            if (StartingScripts != null)
                foreach (var script in StartingScripts)
                    Context.Parse(script, ScriptPath, NugetRepoIdentifier);

            static void AddDefaultLibraryFolderToEnvironmentPath()
            {
                // Explicitly add "Libraries" to PATH
                string path = Path.Combine(AssemblyHelper.ExecutingAssemblyDirectory, "Libraries");
                if (!Directory.Exists(path))
                    path = Path.Combine(Directory.GetCurrentDirectory(), "Libraries");
                if (Directory.Exists(path))
                    AddToEnvironmentPath(path);
            }
        }
        public void Parse(string script)
            => Context.Parse(script, ScriptPath, NugetRepoIdentifier);
        public object Evaluate(string expression)
            => Context.Evaluate(expression, ScriptPath, NugetRepoIdentifier);
        public string GetState()
            => Context.GetState();
        #endregion

        #region Updaters
        public void UpdateScriptFilePath(string scriptFile)
        {
            ScriptPath = scriptFile;
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
                Context.Parse($"""
                    string[] Arguments = new string[{arguments.Length}];
                    """, ScriptPath, NugetRepoIdentifier);
                for (int i = 0; i < arguments.Length; i++)
                {
                    string argument = arguments[i];
                    Context.Parse($"""
                        Arguments[{i}] = @"{argument.Replace("\"", "\\\"")}";
                        """, ScriptPath, NugetRepoIdentifier);
                }
            }
            else
                Context.Parse($"""
                    string[] Arguments = Array.Empty<string>();
                    """, ScriptPath, NugetRepoIdentifier);
        }
        #endregion

        #region Parsing Service
        /// <summary>
        /// Split a large script into executable units for Roslyn
        /// </summary>
        /// <remarks>
        /// Notice due to state issues we can ONLY use this function (when combined with Construct.Parse())
        /// inside hosting contexts that spawns the Interpreter and should NOT use it inside anything that's already being parsed
        /// </remarks>
        public static string[] SplitScripts(string text)
        {
            string[] lines = text
                .Replace("\r", string.Empty)
                .Split('\n')
                .ToArray();
            List<string> scripts = new List<string>();

            bool currentLineIsInBlockComment = false;
            int totalLineCounter = 0;
            StringBuilder scriptBuilder = new();
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                if (!currentLineIsInBlockComment &&
                    (RoslynContext.ImportModuleRegex().IsMatch(line)
                    || RoslynContext.IncludeScriptRegex().IsMatch(line)
                    || RoslynContext.IsolatedScriptLineForSpecialFunctionsRegex().IsMatch(line)   // Remark-cz: This means that our `Parse()` function etc. when executed inside a script can only be on its own line
                    || RoslynContext.HelpItemRegex().IsMatch(line)))
                {
                    if (scriptBuilder.Length != 0)
                    {
                        scripts.Add(new string('\n', totalLineCounter) + scriptBuilder.ToString().Trim());
                        totalLineCounter += string.IsNullOrWhiteSpace(scriptBuilder.ToString()) ? 1 : scriptBuilder.ToString().Split('\n').Length;
                        scriptBuilder.Clear();
                    }
                    scripts.Add(line);
                    totalLineCounter++;
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(line))
                        scriptBuilder.Append("\n");
                    else if (scriptBuilder.Length == 0)
                        scriptBuilder.Append(line);
                    else
                        scriptBuilder.Append("\n" + line);
                }

                // Block comment handling
                if (Regex.Matches(line, @"/\*").Count != Regex.Matches(line, @"\*/").Count) // Remark-cz: This is not robust but it works😆
                    currentLineIsInBlockComment = !currentLineIsInBlockComment;
            }
            if (scriptBuilder.Length > 0)
                scripts.Add(new string('\n', totalLineCounter) + scriptBuilder.ToString().Trim());

            return scripts.ToArray();
        }
        #endregion

        #region Helpers
        public static void AddToEnvironmentPath(string additionalPath)
        {
            Environment.SetEnvironmentVariable("PATH",
                Environment.GetEnvironmentVariable("PATH") != null
                ? (Environment.GetEnvironmentVariable("PATH") + ";" + additionalPath)
                : additionalPath);
        }
        #endregion
    }
}