using Core;

namespace CoreSyntaxUnitTest
{
    public class ScriptParsingTest
    {
        [Fact]
        public void ShouldSplitTextIntoCorrespondingParseableScripts()
        {
            var sections = Parser.SplitScripts("""
                Import(Module)
                Include(Script.cs)
                WriteLine("Hello World!");
                var a = 5;
                void MyFunction()
                {
                    WriteLine("Hello World!");
                }
                """);
            Assert.Equal(3, sections.Length);
            Assert.Equal("Include(Script.cs)", sections[1]);
        }
        [Fact]
        public void ShouldHandleVariableDefinitionsProperly()
        {
            var sections = Parser.SplitScripts("""
                var a = 5;
                var b = someObject
                    .Action1()
                    .Action2();
                WriteLine(a);
                """);
            Assert.Single(sections);
        }
        [Fact]
        public void ShouldHandleLocalFunctionsProperly()
        {
            var sections = Parser.SplitScripts("""
                var a = 5;
                var b = someObject
                    .Action1()
                    .Action2();
                WriteLine(a);
                void MyFunc()
                {
                    void MyLocalFunc()
                    {
                    }
                }
                """);
            Assert.Single(sections);
        }
        [Fact]
        public void ShouldKeepProperLineNumber()
        {
            string script = """
                // Imports
                Import(ModuleA)

                Import(ModuleB)
                using System;
                using System.IO;

                Import(ModuleC)
                using ModuleC;
                using ModuleC.Namespace;

                // Routines
                """;
            var sections = Parser.SplitScripts(script);
            Assert.Equal(script.Split('\n').Length, sections.Last().Split('\n').Length);
        }
        /// <summary>
        /// The reason we had this test is that since we have a few "shorthand" for single-line expressions, it might mess up with those proper C# scripts
        /// </summary>
        [Fact]
        public void ShouldNOTRaiseExceptionsForSingleLineFunctionDefinitions()
        {
            string script = """
                double ComplexFunction(double value, params string[] names){ var n = names; WriteLine(n); return 0; }
                """;
            var sections = Parser.SplitScripts(script);
            Assert.Single(sections);
            Assert.Equal(script, sections.First());

            string output = null;
            Assert.Null(Record.Exception(() => new Interpreter().Start(message => output = message, null, sections)));
            Assert.Null(output);
        }
    }
}