using Core;

namespace CoreTest
{
    public class ScriptParsingTest
    {
        [Fact]
        public void ShouldSplitTextIntoCorrespondingParseableScripts()
        {
            var sections = Interpreter.SplitScripts("""
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
            var sections = Interpreter.SplitScripts("""
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
            var sections = Interpreter.SplitScripts("""
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
            var sections = Interpreter.SplitScripts(script);
            Assert.Equal(script.Split('\n').Length, sections.Last().Split('\n').Length);
        }
        [Fact]
        public void ShouldSupportBlockComments()
        {
            {
                string script = """
                /*This is a block comment that 
                spans multiple lines
                Below are some misleading comment contents:
                var a = 5
                Import(Module)
                void Func(){var b = 16; return;}
                */
                """;
                var sections = Interpreter.SplitScripts(script);
                Assert.Single(sections);
                Assert.Equal(script.Replace("\r\n", "\n"), sections.First());
            }

            {
                string script = """
                var a = /*This is a block comment that 
                spans multiple lines
                Below are some misleading comment contents:
                var a = 5
                Import(Module)
                void Func(){var b = 16; return;}
                */ 5;
                WriteLine(a);
                """;
                var sections = Interpreter.SplitScripts(script);
                Assert.Single(sections);
                Assert.Equal(script.Replace("\r\n", "\n"), sections.First());
            }
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
            var sections = Interpreter.SplitScripts(script);
            Assert.Single(sections);
            Assert.Equal(script, sections.First());

            string output = null;
            Assert.Null(Record.Exception(() => new Interpreter(null, null, null, sections, null).Start(message => output = message)));
            Assert.Null(output);
        }
        [Fact]
        public void ParsingSameComplexScriptShouldWorkConsistently()
        {
            string script = """
                Import(Markdig)

                string Build(string markdown)
                {
                    var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
                    var document = Markdown.ToHtml("This is a text with some *emphasis*", pipeline);
                    return document;
                }
                """;
            var segments = Interpreter.SplitScripts(script);

            Interpreter interpreter = new (null, null, null, null, null);
            interpreter.Start();

            foreach (var a in segments)
                Core.Utilities.Construct.Parse(a);
            Core.Utilities.Construct.Parse("Build(string.Empty)");

            string expected = script.Replace("Import(Markdig)", "string[] Arguments = Array.Empty<string>();").Replace("\r\n", "\n") + "\n" + "Build(string.Empty)";
            Assert.Equal(expected, interpreter.GetState().Replace("\r\n", "\n"));

            foreach (var a in segments)
                Core.Utilities.Construct.Parse(a);
            Assert.Equal(expected + script.Replace("Import(Markdig)", string.Empty).Replace("\r\n", "\n"), interpreter.GetState().Replace("\r\n", "\n"));
        }
        [Fact]
        public void ParsingScriptInsideInterpreterShouldNOTWorkWhenSyntaxDoesntPermit()
        {
            Interpreter interpreter = new(null, null, null, null, null);
            interpreter.Start();

            Assert.Throws<RecursiveParsingException>(() =>
            {
                Core.Utilities.Construct.Parse(""""
                int a = 5; Parse("WriteLine(5)");
                """");
            });
        }
        [Fact]
        public void ParsingScriptInsideInterpreterAsSingleLineShouldWorkOutOfBox()
        {
            Interpreter interpreter = new(null, null, null, null, null);
            interpreter.Start();

            Core.Utilities.Construct.Parse(""""
                Parse("WriteLine(5)");
                """");

            Assert.Equal("""
                string[] Arguments = Array.Empty<string>();
                WriteLine(5)
                """.Replace("\r\n", "\n"), interpreter.GetState().Replace("\r\n", "\n"));
        }
    }
}