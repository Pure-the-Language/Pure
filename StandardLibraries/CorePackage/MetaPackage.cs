namespace CorePackage
{
    /// <summary>
    /// MetaPackage
    /// </summary>
    public abstract class MetaPackage : StandardLibrary
    {
        /// <summary>
        /// Provides retrivable self-identification of package type
        /// </summary>
        public sealed override StandardLibraryType Type { get; } = StandardLibraryType.MetaPackage;
    }
}
