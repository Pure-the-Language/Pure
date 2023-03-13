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
    }
}