using CorePackage;

namespace Data
{
    public class CSV : StandardLibrary
    {
        #region Metadata
        public override StandardLibraryStatus Status => StandardLibraryStatus.Official | StandardLibraryStatus.InDevelopment;
        public override StandardLibraryType Type => StandardLibraryType.Core;
        public override string Name => nameof(CSV);
        public override string Version => "v0.1.0";
        public override string Description => "Simple-to-use CSV-like data (including TSV) reading and writing with optionally strongly typed support.";
        public override string Dependancies => """
            Pure/Core CorePackage
            CsvHelper
            """;
        #endregion
    }
}
