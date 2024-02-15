using Math;

namespace MatrixTest
{
    public class BasicTests
    {
        private const string TestData = """
            Scenario,0,1,2
            0,1,2,3
            1,4,5,6
            2,7,8,9
            """;

        [Fact]
        public void ReadingTest()
        {
            string temp = Path.GetTempFileName();
            File.WriteAllText(temp, TestData);
            Assert.Equal(9, Matrix.Read(temp, true, true)[2][2]);
        }

        [Fact]
        public void WritingTest()
        {
            string temp = Path.GetTempFileName();
            File.WriteAllText(temp, TestData);
            string print = Matrix.Read(temp, true, true).Print("Scenario,0,1,2", row => $"{row}");
            Assert.Equal(TestData, print);
        }
    }
}