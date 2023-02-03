using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using System.Drawing;
using Microsoft.CodeAnalysis.Scripting;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Text;

namespace Core
{
    public class RoslynContext
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
                .AddImports("System.Collections.Generic", "System.Linq");
            if (importAdditional)
                options = options.AddImports("System.Math");
                options = options.AddImports("System.Console");

            State = CSharpScript.RunAsync(string.Empty, options).Result;
        }
        #endregion

        #region Method
        internal void Evaluate(string input)
        {
            string[] special = ParseSpecial(input);
            if (special != null && special.Length != 0)
                foreach (var evaluation in special)
                    EvaluateSingle(evaluation);
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
        }
        #endregion

        #region Routine
        private string[] ParseSpecial(string input)
        {
            // Remark-cz: Notice you might think we can do something similarly to how System.Reflection.Assembly.LoadFrom() works inside the script to load the assembly into the context of the script - indeed that will work for the assembly loading part, but more crucially, we want to import the namespaces as well, and that cannot be done programmatically, and is better done with interpretation.
            var match = Regex.Match(input, @"^Import\((.*?)(, ?(.*?))?\);?$");
            if (match.Success)
            {
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
                    statements.Add($"System.Reflection.Assembly.LoadFrom(@\"{filePath}\");");

                    if (importNamespaces)
                        foreach (var ns in assembly.GetTypes().Where(t => t.IsVisible)
                                .Select(t => t.Namespace).Distinct())
                            statements.Add($"using {ns};");
                    return statements.ToArray();
                }
                else return new string[] { $"WriteLine(\"Cannot find package: {dllName}\")" };
            }
            return null;

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
        }
        private string SyntacWrap(string input)
        {
            // Single line assignment
            if (Regex.IsMatch(input, @"^.*?=.*[^;]$"))
                return $"{input};";
            return input;
        }
        #endregion
    }
}
