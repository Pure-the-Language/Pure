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
                var a = 5
                void MyFunction()
                {
                    WriteLine("Hello World!");
                }
                """);
            Assert.Equal(5, sections.Length);
            Assert.Equal("var a = 5", sections[3]);
            Assert.Equal("""
                void MyFunction()
                {
                    WriteLine("Hello World!");
                }
                """, sections.Last());
        }
        [Fact]
        public void ShouldHandleVariableDefinitionsProperly()
        {
            var sections = Parser.SplitScripts("""
                var a = 5
                var b = someObject
                    .Action1()
                    .Action2()
                WriteLine(a)
                """);
            Assert.Equal(3, sections.Length);
            Assert.Equal("WriteLine(a)", sections[2]);
        }
    }
}