namespace CLITest
{
    public class AdditionalParsingTests
    {
        public struct PositionalArguments
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }
        public struct TypicalScenarioCommandLineArguments
        {
            public bool Help { get; set; }
            public string[] Inputs { get; set; }
            public string[] Outputs { get; set; }
            public int[] Terms { get; set; }
            public float Value { get; set; }
        }

        [Fact]
        public void ShouldParsePositionalArguments()
        {
            Assert.Equal(12, CLI.Main.ParsePositional<PositionalArguments>("Jason", "12").Age);
        }

        [Fact]
        public void ShouldParseTypicalScenario()
        {
            TypicalScenarioCommandLineArguments arguments = CLI.Main.Parse<TypicalScenarioCommandLineArguments>("--help", "--inputs", "Input1.csv", "Input2.csv", "--value", "34");
            Assert.Equal(2, arguments.Inputs.Length);
            Assert.Equal(34, arguments.Value);
        }
    }
}