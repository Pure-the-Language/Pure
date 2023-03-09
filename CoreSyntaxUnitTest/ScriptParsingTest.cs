using Core;
using NuGet.ContentModel;

namespace CoreSyntaxUnitTest
{
    public class ScriptParsingTest
    {
        [Fact]
        public void ShouldSplitTextIntoCorrespondingParseableScripts()
        {
            int sections = Parser.SplitScripts("""
                Import(Module)
                Include(Script.cs)
                WriteLine("Hello World!");
                void MyFunction()
                {
                    WriteLine("Hello World!");
                }
                """).Length;
            Asset.Equals(4, sections);
        }
    }
}