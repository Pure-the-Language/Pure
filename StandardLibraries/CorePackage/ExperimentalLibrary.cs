namespace CorePackage
{
    /// <summary>
    /// In-dev library
    /// </summary>
    public abstract class ExperimentalLibrary: StandardLibrary
    {
        /// <summary>
        /// Provides retrivable self-identification of package type
        /// </summary>
        public sealed override StandardLibraryType Type { get; } = StandardLibraryType.Experimental;
    }
}
