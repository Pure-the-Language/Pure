using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using System.Drawing;
using Microsoft.CodeAnalysis.Scripting;
using System.Text.RegularExpressions;
using System.Reflection;

namespace Core
{
    public partial class RoslynContext
    {
        #region Private States
        private ScriptState<object> State { get; set; }
        private static RoslynContext _Singleton;
        public static RoslynContext Singleton => _Singleton;
        #endregion

        #region Construction
        public RoslynContext(bool importAdditional)
        {
            if (_Singleton != null)
                throw new InvalidOperationException("Roslyn Context is already initialized.");
            _Singleton = this;

            ScriptOptions options = ScriptOptions.Default
                // Remark-cz: Use Add* instead of With* to add stuff instead of replacing stuff
                .WithReferences(typeof(RoslynContext).Assembly)
                .AddReferences(typeof(Enumerable).Assembly)
                .AddImports("System.Collections.Generic", "System.Linq")
                .AddImports("Core.Math")
                .AddImports("Core.Utilities")
                .WithImports("Core.Construct");
            if (importAdditional)
                options = options.AddImports("System.Math");
                options = options.AddImports("System.Console");

            State = CSharpScript.RunAsync(string.Empty, options).Result;
        }
        #endregion

        #region Method
        internal void Evaluate(string input)
        {
            if (input.TrimStart().StartsWith('#'))
                return; // Skip line-style comment
            else if (ImportModuleRegex().IsMatch(input))
            {
                // Remark-cz: Notice you might think we can do something similarly to how System.Reflection.Assembly.LoadFrom() works inside the script to load the assembly into the context of the script - indeed that will work for the assembly loading part, but more crucially, we want to import the namespaces as well, and that cannot be done programmatically, and is better done with interpretation.
                var match = ImportModuleRegex().Match(input);
                string dllName = match.Groups[1].Value.Trim('"');
                bool importNamespaces = !string.IsNullOrWhiteSpace(match.Groups[2].Value)
                    ? bool.Parse(match.Groups[4].Value.ToLower())
                    : true;

                string filePath = dllName;
                if (!File.Exists(dllName))
                    filePath = TryFindDLLFile(dllName);

                List<string> statements = new List<string>();
                if (filePath != null && File.Exists(filePath))
                {
                    Assembly assembly = Assembly.LoadFrom(filePath); // Might load from within the Roslyn state's context//app domain?
                    AddReference(assembly);

                    if (importNamespaces)
                        foreach (var ns in assembly.GetTypes().Where(t => t.IsVisible)
                                .Select(t => t.Namespace).Distinct())
                            AddImport(ns);
                    var mainType = assembly.GetTypes().FirstOrDefault(t => t.Name == "Main" && t.IsVisible && t.IsAbstract && t.IsSealed);
                    if (mainType != null)
                        AddImport($"{mainType.Namespace}.Main");
                }
                else Console.WriteLine($"WriteLine(\"Cannot find package: {dllName}\")");

                return;
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
                    State = State.ContinueWithAsync(SyntacWrap(singleEvaluation)).Result;
                    if (State.ReturnValue != null)
                        Console.WriteLine(State.ReturnValue);
                }
                catch (Exception e)
                {
                    e = e.InnerException ?? e;
                    Console.WriteLine(Regex.Replace(e.Message, @"error CS\d\d\d\d: ", string.Empty), Color.Red);
                }
            }

            static string TryFindDLLFile(string dllName)
            {
                foreach (var path in Environment.GetEnvironmentVariable("PATH")
                .Split(';', StringSplitOptions.RemoveEmptyEntries))
                {
                    try
                    {
                        foreach (var file in Directory.EnumerateFiles(path))
                        {
                            string fileName = Path.GetFileName(file);
                            string fileNameNoExtention = Path.GetFileNameWithoutExtension(file);
                            string extension = Path.GetExtension(file).ToLower();
                            if (extension == ".dll" || extension == ".exe")
                            {
                                if (fileName == dllName || fileNameNoExtention == dllName)
                                    return file;
                            }
                        }
                    }
                    // Remark-cz: Certain paths might NOT be enumerable due to access issues
                    catch (Exception) { continue; }
                }
                return null;
            }
            void AddReference(Assembly assembly)
                => State = State.ContinueWithAsync(string.Empty, State.Script.Options.AddReferences(assembly)).Result;
            void AddImport(string import)
                => State = State.ContinueWithAsync(string.Empty, State.Script.Options.AddImports(import)).Result;
        }
        #endregion

        #region Routine
        private static string SyntacWrap(string input)
        {
            // Numerical array data
            var match = VariableCreationRegex().Match(input);
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
                var type = AppDomain.CurrentDomain
                    .GetAssemblies()
                    .SelectMany(a => a.GetTypes().Where(t => t.Name == name)).Single();
                PrintType(type);
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
                .Select(m =>
                {
                    string name = m.Name;
                    string[] arguments = m.GetParameters()
                        .Select(p => $"{p.ParameterType.Name} {p.Name}")
                        .ToArray();
                    return $"{name}({string.Join(", ", arguments)})";
                })
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
        #endregion

        #region Regex
        [GeneratedRegex(@"^Import\((.*?)(, ?(.*?))?\);?$")]
        private static partial Regex ImportModuleRegex();
        [GeneratedRegex(@"^Help\((.*?)\)$")]
        private static partial Regex HelpItemRegex();
        [GeneratedRegex(@"^var ([^ ]+?) *= *\[(.*?)\] *$")]
        private static partial Regex VariableCreationRegex();
        [GeneratedRegex("^.*?=.*[^;]$")]
        private static partial Regex LineAssignmentRegex();
        #endregion
    }
}
