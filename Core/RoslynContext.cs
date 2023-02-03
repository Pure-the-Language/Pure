using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using System.Drawing;
using Microsoft.CodeAnalysis.Scripting;
using System.Text.RegularExpressions;

namespace Core
{
    public class RoslynContext
    {
        #region Private States
        private ScriptState<object> State { get; set; }
        #endregion

        #region Construction
        public RoslynContext()
            : this(null) { }
        public RoslynContext(ScriptOptions options)
        {
            options = options ?? ScriptOptions.Default;
            options = options
                .WithReferences(typeof(RoslynContext).Assembly)
                .WithImports("Core.Construct");

            State = CSharpScript.RunAsync(string.Empty, options).Result;
        }
        #endregion

        #region Properties

        #endregion

        #region Method
        internal void Evaluate(string input)
        {
            try
            {
                State = State.ContinueWithAsync(SyntacWrap(input)).Result;
                if (State.ReturnValue != null)
                    Console.WriteLine(State.ReturnValue);
            }
            catch (Exception e)
            {
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
