using Core;

namespace CoreTest
{
    public class IncludeTest
    {
        [Fact]
        public void IncludeShouldWorkWithPathRelativeToScriptFile()
        {
            string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);
            string tempFile = Path.Combine(tempDirectory, "Script.cs");
            string outputFile = Path.Combine(tempDirectory, "Output.txt");

            string libraryFile = Path.Combine(tempDirectory, "Library.cs");
            File.WriteAllText(libraryFile, $"""
                File.WriteAllText(@"{outputFile}", "Hello World");
                """);

            new Interpreter(null, tempFile, null, new[] { 
                """
                Include(Library.cs)
                """}, null).Start();
            Assert.Equal("Hello World", File.ReadAllText(outputFile));
        }
    }
}
