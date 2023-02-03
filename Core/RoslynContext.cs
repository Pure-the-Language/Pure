using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using System.Drawing;
using Microsoft.CodeAnalysis.Scripting;
using System.Text.RegularExpressions;
using System.Reflection;

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
                .WithImports("Core.Construct")
                .AddImports("System.Collections.Generic", "System.Linq");
            if (importAdditional)
                options = options.AddImports("System.Math");
                options = options.AddImports("System.Console");

            State = CSharpScript.RunAsync(string.Empty, options).Result;
        }
        #endregion

        #region State Manipulation
        public void AddReference(Assembly assembly)
            => State = State.ContinueWithAsync(string.Empty, State.Script.Options.AddReferences(assembly)).Result;
        public void AddImport(string import)
            => State = State.ContinueWithAsync(string.Empty, State.Script.Options.AddImports(import)).Result;
        #endregion

        #region Method
        internal void Evaluate(string input)
        {
            try
            {
                // Remark-cz: This will NOT work with actions that modifies state by calling host functions
                // Basically, host functions cannot and should not have side-effects on the state object directly
                State = State.ContinueWithAsync(SyntacWrap(input)).Result;
                if (State.ReturnValue != null)
                    Console.WriteLine(State.ReturnValue);
            }
            catch (Exception e)
            {
                e = e.InnerException ?? e;
                Console.WriteLine(Regex.Replace(e.Message, @"error CS\d\d\d\d: ", string.Empty), Color.Red);
            }
        }
        public void Compile(string source)
        {
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source);
            CSharpCompilation compilation = CSharpCompilation.Create(
                "assemblyName",
                new[] { syntaxTree },
                new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) },
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using (var dllStream = new MemoryStream())
            using (var pdbStream = new MemoryStream())
            {
                var emitResult = compilation.Emit(dllStream, pdbStream);
                if (!emitResult.Success)
                {
                    // emitResult.Diagnostics
                }
            }
        }
        #endregion

        #region Routine
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
