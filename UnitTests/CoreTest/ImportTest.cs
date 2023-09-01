using Core;

namespace CoreTest
{
    public class ImportTest
    {
        [Fact]
        public void ImportShouldBeAbleToDownloadNugetsAutomatically()
        {
            new Interpreter(null, null, null, new[] {
                """
                Import(Newtonsoft.Json)
                Help(Newtonsoft.Json)
                """}, "LibraryImportTest").Start();
            // If it passes through without problem, it's good
        }
    }
}
