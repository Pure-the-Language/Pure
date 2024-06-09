namespace CorePackage
{
    /// <summary>
    /// Framework
    /// </summary>
    public abstract class Framework : StandardLibrary
    {
        /// <summary>
        /// Provides retrivable self-identification of package type
        /// </summary>
        public sealed override StandardLibraryType Type { get; } = StandardLibraryType.Framework;
    }
}
