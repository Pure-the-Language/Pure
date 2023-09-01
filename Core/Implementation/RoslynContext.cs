using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Text;
using System.Diagnostics.CodeAnalysis;
using Core.Services;
using Core.Utilities;
using Core.Helpers;

namespace Core
{
    internal sealed class RedirectedTextWriter : TextWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
        public override void Write(bool value)
        {
            RoslynContext.OutputHandler.Invoke(value.ToString());
        }
        public override void Write(char value)
        {
            RoslynContext.OutputHandler.Invoke(value.ToString());
        }
        public override void Write(char[] buffer, int index, int count)
        {
            RoslynContext.OutputHandler.Invoke(new string(buffer.Skip(index).Take(count).ToArray()));
        }
        public override void Write(char[] buffer)
        {
            RoslynContext.OutputHandler.Invoke(new string(buffer));
        }
        public override void Write(decimal value)
        {
            RoslynContext.OutputHandler.Invoke(value.ToString());
        }
        public override void Write(double value)
        {
            RoslynContext.OutputHandler.Invoke(value.ToString());
        }
        public override void Write(float value)
        {
            RoslynContext.OutputHandler.Invoke(value.ToString());
        }
        public override void Write(int value)
        {
            RoslynContext.OutputHandler.Invoke(value.ToString());
        }
        public override void Write(long value)
        {
            RoslynContext.OutputHandler.Invoke(value.ToString());
        }
        public override void Write(object value)
        {
            RoslynContext.OutputHandler.Invoke(value.ToString());
        }
        public override void Write(ReadOnlySpan<char> buffer)
        {
            RoslynContext.OutputHandler.Invoke(buffer.ToString());
        }
        public override void Write([StringSyntax("CompositeFormat")] string format, object arg0)
        {
            RoslynContext.OutputHandler.Invoke(string.Format(format, arg0));
        }
        public override void Write([StringSyntax("CompositeFormat")] string format, object arg0, object arg1)
        {
            RoslynContext.OutputHandler.Invoke(string.Format(format, arg0, arg1));
        }
        public override void Write([StringSyntax("CompositeFormat")] string format, object arg0, object arg1, object arg2)
        {
            RoslynContext.OutputHandler.Invoke(string.Format(format, arg0, arg1, arg2));
        }
        public override void Write([StringSyntax("CompositeFormat")] string format, params object[] arg)
        {
            RoslynContext.OutputHandler.Invoke(string.Format(format, arg));
        }
        public override void Write(string value)
        {
            RoslynContext.OutputHandler.Invoke(value.ToString());
        }
        public override void Write(StringBuilder value)
        {
            RoslynContext.OutputHandler.Invoke(value.ToString());
        }
        public override void Write(uint value)
        {
            RoslynContext.OutputHandler.Invoke(value.ToString());
        }
        public override void Write(ulong value)
        {
            RoslynContext.OutputHandler.Invoke(value.ToString());
        }
        public override void WriteLine()
        {
            RoslynContext.OutputHandler.Invoke(Environment.NewLine);
        }
        public override void WriteLine(bool value)
        {
            RoslynContext.OutputHandler.Invoke(value.ToString() + Environment.NewLine);
        }
        public override void WriteLine(char value)
        {
            RoslynContext.OutputHandler.Invoke(value.ToString() + Environment.NewLine);
        }
        public override void WriteLine(char[] buffer, int index, int count)
        {
            RoslynContext.OutputHandler.Invoke(new string(buffer.Skip(index).Take(count).ToArray()) + Environment.NewLine);
        }
        public override void WriteLine(char[] buffer)
        {
            RoslynContext.OutputHandler.Invoke(new string(buffer) + Environment.NewLine);
        }
        public override void WriteLine(decimal value)
        {
            RoslynContext.OutputHandler.Invoke(value.ToString() + Environment.NewLine);
        }
        public override void WriteLine(double value)
        {
            RoslynContext.OutputHandler.Invoke(value.ToString() + Environment.NewLine);
        }
        public override void WriteLine(float value)
        {
            RoslynContext.OutputHandler.Invoke(value.ToString() + Environment.NewLine);
        }
        public override void WriteLine(int value)
        {
            RoslynContext.OutputHandler.Invoke(value.ToString() + Environment.NewLine);
        }
        public override void WriteLine(long value)
        {
            RoslynContext.OutputHandler.Invoke(value.ToString() + Environment.NewLine);
        }
        public override void WriteLine(object value)
        {
            RoslynContext.OutputHandler.Invoke(value.ToString() + Environment.NewLine);
        }
        public override void WriteLine(ReadOnlySpan<char> buffer)
        {
            RoslynContext.OutputHandler.Invoke(buffer.ToString() + Environment.NewLine);
        }
        public override void WriteLine([StringSyntax("CompositeFormat")] string format, object arg0)
        {
            RoslynContext.OutputHandler.Invoke(string.Format(format, arg0) + Environment.NewLine);
        }
        public override void WriteLine([StringSyntax("CompositeFormat")] string format, object arg0, object arg1)
        {
            RoslynContext.OutputHandler.Invoke(string.Format(format, arg0, arg1) + Environment.NewLine);
        }
        public override void WriteLine([StringSyntax("CompositeFormat")] string format, object arg0, object arg1, object arg2)
        {
            RoslynContext.OutputHandler.Invoke(string.Format(format, arg0, arg1, arg2) + Environment.NewLine);
        }
        public override void WriteLine([StringSyntax("CompositeFormat")] string format, params object[] arg)
        {
            RoslynContext.OutputHandler.Invoke(string.Format(format, arg) + Environment.NewLine);
        }
        public override void WriteLine(string value)
        {
            RoslynContext.OutputHandler.Invoke(value + Environment.NewLine);
        }
        public override void WriteLine(StringBuilder value)
        {
            RoslynContext.OutputHandler.Invoke(value.ToString() + Environment.NewLine);
        }
        public override void WriteLine(uint value)
        {
            RoslynContext.OutputHandler.Invoke(value.ToString() + Environment.NewLine);
        }
        public override void WriteLine(ulong value)
        {
            RoslynContext.OutputHandler.Invoke(value.ToString() + Environment.NewLine);
        }
    }
    /// <remarks>
    /// This is implementation detail and should be protected from external users
    /// </remarks>
    internal partial class RoslynContext
    {
        #region Private States
        private List<string> ImportedModules { get; set; } = new List<string>();
        private ScriptState<object> State { get; set; }
        /// <summary>
        /// Remark-cz: We require RoslynContext to be a singleton for some good reason - what was it?
        /// (There are a few likely reasons, pending further investigation: 
        /// 1. We touched process level env variable modifications; 
        /// 2. We are piping STD OUT through custom handler;
        /// 3. We are binding to AppDomain.CurrentDomain.ProcessExit.)
        /// </summary>
        private static RoslynContext _Singleton;
        /// <summary>
        /// Remark-cz: We are hiding this singleton because we want to avoid using it; When we figure out why we needed singleton in the first place, we might be able to not require a singleton patter nat all
        /// </summary>
        private static RoslynContext Singleton => _Singleton;
        #endregion

        #region Runtime Configurable Behaviors
        /// <summary>
        /// Configures the output handling to use
        /// </summary>
        public static Action<string> OutputHandler;
        #endregion

        #region Lifetime Event
        private Action ShutdownEvents { get; set; }
        private void CurrentDomainProcessExit(object sender, EventArgs e)
        {
            ShutdownEvents?.Invoke();
        }
        #endregion

        #region Construction
        public RoslynContext(bool importAdditional, Action<string> outputHandler)
        {
            if (_Singleton != null)
                throw new InvalidOperationException("Roslyn Context is already initialized.");
            _Singleton = this;
            if (outputHandler != null)
            {
                OutputHandler = outputHandler;
                Console.SetOut(new RedirectedTextWriter());
            }
            // Bind Process Exit Event
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomainProcessExit);

            ScriptOptions options = ScriptOptions.Default
                // Remark-cz: Use Add* instead of With* to add stuff instead of replacing stuff
                .WithReferences(typeof(RoslynContext).Assembly)
                .AddReferences(typeof(Enumerable).Assembly)
                .AddImports(
                    "System",
                    "System.Collections.Generic",
                    "System.IO", 
                    "System.Linq")
                // Pure language essential namespaces, types, and global static functions
                .AddImports($"{nameof(Core)}.{nameof(Math)}")
                .AddImports($"{nameof(Core)}.{nameof(Utilities)}")
                .AddImports($"{nameof(Core)}.{nameof(Utilities)}.{nameof(Construct)}");
            // Additional commonly used but secondary imports
            if (importAdditional)
            {
                // Commonly used .Net namespace
                options = options.AddImports("System.Math");
                options = options.AddImports("System.Console");
                // Pure core standard libraries
                AddDefaultLibraryFolderToEnvironmentPath();
            }

            State = CSharpScript.RunAsync(string.Empty, options).Result;

            static void AddDefaultLibraryFolderToEnvironmentPath()
            {
                // Remark-cz: The Core will be decoupled better if we don't rely on such env variables so explicitly, or at least don't modify it this way
                // TODO: We should probably just pass in during context/interpreter initialization some external "additional" library folders instead of explicitly search for it in Core. Instead, we can search for it and initialize it as an optional option in Interpreter, which is shared by both Pure (REPL) and Notebook
                string path = Path.Combine(AssemblyHelper.AssemblyDirectory, "Libraries");
                if (!Directory.Exists(path))
                    path = Path.Combine(Directory.GetCurrentDirectory(), "Libraries");
                if (Directory.Exists(path))
                    Environment.SetEnvironmentVariable("PATH", 
                        Environment.GetEnvironmentVariable("PATH") != null 
                        ? (Environment.GetEnvironmentVariable("PATH") + ";" + path)
                        : path);
            }
        }
        #endregion

        #region Method
        internal void Evaluate(string input, string currentScriptFile, string nugetRepoIdentifier)
        {
            if (input.TrimStart().StartsWith('#'))
                return; // Skip line-style comment
            else if (ImportModuleRegex().IsMatch(input))
            {
                // Remark-cz: Notice you might think we can do something similarly to how System.Reflection.Assembly.LoadFrom() works inside the script to load the assembly into the context of the script - indeed that will work for the assembly loading part, but more crucially, we want to import the namespaces as well, and that cannot be done programmatically, and is better done with interpretation.
                var match = ImportModuleRegex().Match(input);
                string dllName = match.Groups[1].Value.Trim('"');
                bool importNamespaces = string.IsNullOrWhiteSpace(match.Groups[2].Value) 
                    || bool.Parse(match.Groups[4].Value.ToLower());

                string filePath = dllName;
                if (!File.Exists(dllName))
                    filePath = TryFindDLLFile(dllName, nugetRepoIdentifier);

                List<string> statements = new();
                if (filePath != null && File.Exists(filePath))
                {
                    if (ImportedModules.Contains(filePath)) return;
                    else ImportedModules.Add(filePath);

                    // Remark-cz: Add the moment this fails to deal with most scenarios when the package is NOT properly including the single runtime i.e. there is a "runtimes" folder which contains corresponding runtimes (The Target runtime is selected as "Portable" instead of specifc runtime). In this case it will say "<Some module> is not supported on this platform" when the module is actually available in the published build.
                    // Potential reference: https://stackoverflow.com/questions/1373100/how-to-add-folder-to-assembly-search-path-at-runtime-in-net
                    Assembly assembly = Assembly.LoadFrom(filePath); // Remark: Might load from within the Roslyn state's context//app domain?
                    AddReference(assembly);

                    if (importNamespaces)
                        foreach (var ns in assembly.GetTypes().Where(t => t.IsVisible)
                                .Select(t => t.Namespace).Distinct())
                            AddImport(ns);
                    // Special handle Main class
                    Type mainType = assembly.GetTypes().FirstOrDefault(t => t.Name == "Main" && t.IsVisible && t.IsAbstract && t.IsSealed);
                    if (mainType != null)
                    {
                        // Expose all functions at top level for the "Main" interface
                        AddImport($"{mainType.Namespace}.Main");
                        // Execute "StartUp" if any
                        MethodInfo startUp = mainType.GetMethod("StartUp", BindingFlags.NonPublic | BindingFlags.Static);
                        if (startUp != null && startUp.GetParameters().Length == 0 && startUp.IsPrivate == true)
                            startUp.Invoke(null, null);
                        // Handle "ShutDown" if any
                        MethodInfo shutDown = mainType.GetMethod("ShutDown", BindingFlags.NonPublic | BindingFlags.Static);
                        if (shutDown != null && shutDown.GetParameters().Length == 0 && shutDown.IsPrivate == true)
                            ShutdownEvents += (Action)Delegate.CreateDelegate(typeof(Action), shutDown);
                    }
                }
                else Console.WriteLine($"Cannot find package: {dllName}");

                return;
            }
            else if (IncludeScriptRegex().IsMatch(input))
            {
                // Remark-cz: Include search order: Current working directory, script file path (if any), PUREPATH
                var match = IncludeScriptRegex().Match(input);
                string scriptName = match.Groups[1].Value.Trim('"');
                
                string scriptPath = scriptName;
                if (!File.Exists(scriptName))
                    scriptPath = TryFindScriptFile(scriptName, currentScriptFile);

                if (!File.Exists(scriptPath))
                    throw new ArgumentException($"File {scriptPath ?? scriptName} doesn't exist.");

                string text = File.ReadAllText(scriptPath);
                foreach (var code in Interpreter.SplitScripts(text))
                    Evaluate(code, scriptPath, nugetRepoIdentifier);
            }
            else if (HelpItemRegex().IsMatch(input))
            {
                var match = HelpItemRegex().Match(input);
                string name = match.Groups[1].Value;
                bool isPrintMetaData = PrintName(name);
                if (!isPrintMetaData)
                    EvaluateSingle(input);
            }
            else EvaluateSingle(input);

            // Remark-cz: Things like "using" statement cannot be put in the middle of code block like other statements and require special treatment
            void EvaluateSingle(string singleEvaluation)
            {
                try
                {
                    // Remark-cz: This will NOT work with actions that modifies state by calling host functions
                    // Basically, host functions cannot and should not have side-effects on the state object directly
                    State = State.ContinueWithAsync(SyntaxWrap(singleEvaluation)).Result;
                    if (State.ReturnValue != null)
                        PrintReturnValuePreviews(State.ReturnValue);
                }
                catch (Exception e)
                {
                    e = e.InnerException ?? e;
                    Console.WriteLine(Regex.Replace(e.Message, @"error CS\d\d\d\d: ", string.Empty));
                    if (e is not ApplicationException && e is not CompilationErrorException)
                            Console.WriteLine(e.StackTrace);
                }
            }
            void AddReference(Assembly assembly)
                => State = State.ContinueWithAsync(string.Empty, State.Script.Options.AddReferences(assembly)).Result;
            void AddImport(string import)
                => State = State.ContinueWithAsync(string.Empty, State.Script.Options.AddImports(import)).Result;
            void PrintReturnValuePreviews(object returnValue)
            {
                // Print string items preview
                if (returnValue is System.Collections.IList list)
                {
                    Console.WriteLine($"{returnValue} (Count: {list.Count})");
                    if (list.Count < 10)
                        foreach (var item in list)
                            Console.WriteLine(item.ToString());
                    else
                    {
                        for (int i = 0; i < 7; i++)
                            Console.WriteLine(list[i].ToString());
                        Console.WriteLine(".");
                        Console.WriteLine(".");
                        Console.WriteLine(".");
                        for (int i = 3; i > 0; i--)
                            Console.WriteLine(list[list.Count - i].ToString());
                    }
                }
                // Print general preview of primitives and type
                else
                    Console.WriteLine(returnValue.ToString());
            }
        }
        #endregion

        #region Helpers
        public static string TryFindDLLFile(string dllName, string nugetRepoIdentifier)
        {
            // Try find from PATH
            string path = PathHelper.FindDLLFileFromEnvPath(dllName);
            if (path != null)
                return path;
            // Try downloading from Nuget
            return QuickEasyDirtyNugetPreparer.TryDownloadNugetPackage(dllName, nugetRepoIdentifier);
        }
        public static string TryFindScriptFile(string scriptName, string currentScriptFile)
        {
            string fullpath = Path.GetFullPath(scriptName);
            if (File.Exists(fullpath))
                return fullpath;
            if (currentScriptFile != null)
            {
                string currentScriptFileFolder = Path.GetDirectoryName(Path.GetFullPath(currentScriptFile));
                string absolutePathRelativeToCurrentScript = Path.Combine(currentScriptFileFolder, scriptName);
                if (Path.Exists(absolutePathRelativeToCurrentScript))
                    return absolutePathRelativeToCurrentScript;
            }
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("PUREPATH")))
                return null;
            foreach (var path in Environment.GetEnvironmentVariable("PUREPATH")
                .Split(';', StringSplitOptions.RemoveEmptyEntries))
            {
                try
                {
                    foreach (var file in Directory.EnumerateFiles(path))
                    {
                        string fileName = Path.GetFileName(file);
                        string fileNameNoExtention = Path.GetFileNameWithoutExtension(file);
                        string extension = Path.GetExtension(file).ToLower();
                        if (extension == ".pure")
                        {
                            if (fileName == scriptName || fileNameNoExtention == scriptName)
                                return file;
                        }
                    }
                }
                // Remark-cz: Certain paths might NOT be enumerable due to access issues
                catch (Exception) { continue; }
            }
            return null;
        }
        #endregion

        #region Routine
        private static string SyntaxWrap(string input)
        {
            // Numerical array creation  // Remark-cz-2023-Aug: This is troublesome, consider deprecate this feature.
            var match = ArrayVariableCreationRegex().Match(input);
            if (match.Success)
            {
                string varName = match.Groups[1].Value;
                string[] values = match.Groups[2].Value
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(v => v.Trim())
                    .Where(v => !string.IsNullOrWhiteSpace(v))
                    .ToArray();
                if (values.All(v => double.TryParse(v, out _)))
                    input = $"var {varName} = new Vector(new double[] {{{string.Join(",", values)}}})";
            }
            // Literal numerical array  // Remark-cz-2023-Aug: This is troublesome, consider deprecate this feature.
            input = LiteralArrayRegex().Replace(input, m =>
            {
                if (m.Groups["Numeral"].Captures.All(c => double.TryParse(c.Value, out _)))
                    return $"new double[]{{{string.Join(", ", m.Groups["Numeral"].Captures.Select(c => c.Value))}}}";
                return m.Value;
            });
            // Single line assignment
            if (LineAssignmentRegex().IsMatch(input))
                return $"{input};";
            return input;
        }
        #endregion

        #region Helpers
        public static bool PrintName(string name)
        {
            string[] nameSpaces = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes().Select(t => t.Namespace)).Distinct().ToArray();
            string[] types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes().Select(t => t.Name)).Distinct().ToArray();
            string[] typesFullname = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes().Select(t => t.FullName)).Distinct().ToArray();

            if (nameSpaces.Contains(name))
            {
                var subNamespaces = AppDomain.CurrentDomain
                    .GetAssemblies()
                    .SelectMany(a =>
                        a.GetTypes()
                        .Where(t => t.Namespace?.StartsWith($"{name}.") ?? false)
                        .Select(t => t.Namespace)
                    )
                    .Distinct()
                    .OrderBy(t => t)
                    .ToArray();
                var publicTypes = AppDomain.CurrentDomain
                    .GetAssemblies()
                    .SelectMany(a =>
                        a.GetTypes()
                        .Where(t => t.Namespace == name)
                        .Select(t => t.Name)
                        .Where(n => !n.StartsWith("<>")) // Skip internal names
                    )
                    .Distinct()
                    .OrderBy(t => t)
                    .ToArray();
                Console.WriteLine($"""
                        Namespace: {name}
                        Namespaces: 
                          {string.Join(Environment.NewLine + "  ", subNamespaces)}
                        Types: 
                          {string.Join(Environment.NewLine + "  ", publicTypes)}
                        """);
                return true;
            }
            else if (types.Contains(name))
            {
                var foundTypes = AppDomain.CurrentDomain
                    .GetAssemblies()
                    .SelectMany(a => a.GetTypes().Where(t => t.Name == name)).ToArray();
                if (foundTypes.Length == 1)
                    PrintType(foundTypes.Single());
                else
                    Console.WriteLine(string.Join('\n', foundTypes.Select(t => t.FullName)));
                return true;
            }
            else if (typesFullname.Contains(name))
            {
                var type = AppDomain.CurrentDomain
                    .GetAssemblies()
                    .SelectMany(a => a.GetTypes().Where(t => t.FullName == name)).Single();
                PrintType(type);
                return true;
            }
            else if (AppDomain.CurrentDomain.GetAssemblies().Any(a => a.GetTypes().Any(t => t.GetMethods().Any(m => m.Name == name))))
            {
                MethodInfo methodInfo = null;
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                    foreach (var type in assembly.GetTypes())
                        foreach (var method in type.GetMethods())
                            if (method.Name == name)
                                methodInfo = method;
                if (methodInfo != null)
                    Console.WriteLine(PrintMethod(methodInfo));
                return true;
            }
            return false;
        }
        public static void PrintType(Type type)
        {
            var publicFields = type
                .GetFields()
                .Select(m => m.Name)
                .Distinct()
                .OrderBy(t => t)
                .ToArray();
            var publicProperties = type
                .GetProperties()
                .Select(m => m.Name)
                .Distinct()
                .OrderBy(t => t)
                .ToArray();
            var publicMethods = type
                .GetMethods()
                .Select(PrintMethod)
                .Distinct()
                .OrderBy(t => t)
                .ToArray();
            Console.WriteLine($"""
                Type: {type.Name}
                """);
            if (publicFields.Length > 0)
                Console.WriteLine($"""
                    Fields: 
                      {string.Join(Environment.NewLine + "  ", publicFields)}
                    """);
            if (publicProperties.Length > 0)
                Console.WriteLine($"""
                    Properties: 
                      {string.Join(Environment.NewLine + "  ", publicProperties)}
                    """);
            if (publicMethods.Length > 0)
                Console.WriteLine($"""
                    Methods: 
                      {string.Join(Environment.NewLine + "  ", publicMethods)}
                    """);
        }
        public static string PrintMethod(MethodInfo m)
        {
            string name = m.Name;
            string[] arguments = m.GetParameters()
                .Select(p => $"{p.ParameterType.Name} {p.Name}")
                .ToArray();
            string returnType = m.ReturnType == typeof(void) ? string.Empty : $" : {m.ReturnType.Name}";
            if (m.ReturnType.GenericTypeArguments.Length != 0)
            {
                var generics = m.ReturnType.GetGenericArguments().Select(t => t.Name);
                returnType = $" : {m.ReturnType.Name}<{string.Join(", ", generics)}>";
            }
            if (m.ContainsGenericParameters)
            {
                var generics = m.GetGenericArguments().Select(t => t.Name);
                return $"{name}<{string.Join(", ", generics)}>({string.Join(", ", arguments)}){returnType}";
            }
            else 
                return $"{name}({string.Join(", ", arguments)}){returnType}";
        }
        #endregion

        #region Regex
        [GeneratedRegex(@"^Import\((.*?)(, ?(.*?))?\);?$")]
        public static partial Regex ImportModuleRegex();
        [GeneratedRegex(@"^Include\((.*?)(, ?(.*?))?\);?$")]
        public static partial Regex IncludeScriptRegex();
        [GeneratedRegex(@"^Help\((.*?)\)$")]
        public static partial Regex HelpItemRegex();
        [GeneratedRegex(@"^var ([^ ]+?) *= *\[(.*?)\] *$")]
        public static partial Regex ArrayVariableCreationRegex();
        [GeneratedRegex(@"(?<=[^a-zA-Z0-9])\[((?<Numeral>\d+),\s*?){2,}((?<Numeral>\d+)\s*?)\]")]
        public static partial Regex LiteralArrayRegex();
        [GeneratedRegex(@"^(\S*)?\s*[a-zA-Z0-9_]+\s*=.*[^;]$")]
        public static partial Regex LineAssignmentRegex();
        #endregion
    }
}
