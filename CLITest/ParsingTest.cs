namespace CLITest
{
    public class ParsingTest
    {
        public struct TestStruct001
        {
            public string Name;
            public int Age;
        }
        public struct PositionalArguments
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }
        public record TestRecord001(string InputPath, string[] Outputs, bool SkipFirstLine);

        [Fact]
        public void ShouldParseStructsWithFields()
        {
            Assert.Equal(12, CLI.Main.Parse<TestStruct001>("--Name", "Jason", "--Age", "12").Age);
        }

        [Fact]
        public void ShouldParseClassWithProperties()
        {
            Assert.Equal(12, CLI.Main.Parse<PositionalArguments>("--Name", "Jason", "--Age", "12").Age);
        }

        [Fact]
        public void ShouldParseRecords()
        {
            Assert.False(CLI.Main.Parse<TestRecord001>("--InputPath", "Input.csv", "--SkipFirstLine", "False").SkipFirstLine);
        }

        [Fact]
        public void ParsingShouldBeCaseInsensitive()
        {
            var p1 = CLI.Main.Parse<TestRecord001>("--inputPath", "Input.csv", "--skipFirstLine", "False");
            var p2 = CLI.Main.Parse<TestRecord001>("--InputPath", "Input.csv", "--SkipFirstLine", "True");

            Assert.False(p1.SkipFirstLine);
            Assert.True(p2.SkipFirstLine);
        }

        [Fact]
        public void NotAllPropertiesNeedToBeProvided()
        {
            var parse = CLI.Main.Parse<TestRecord001>("--inputPath", "Input.csv", "--skipFirstLine", "False");

            Assert.Empty(parse.Outputs);
        }

        [Fact]
        public void ShouldWarnAboutInvalidKeys()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                CLI.Main.Parse<TestRecord001>("--inputPath", "Input.csv", "--UnknownKey");
            });
        }

        [Fact]
        public void ArraysWillBeInitialized()
        {
            Assert.NotNull(CLI.Main.Parse<TestRecord001>().Outputs);
        }
    }
}