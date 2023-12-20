namespace CorePackage
{
    /// <summary>
    /// Development status of library
    /// </summary>
    [Flags]
    public enum StandardLibraryStatus
    {
        /// <summary>
        /// The library if official and can be used in production
        /// </summary>
        Official = 1,
        /// <summary>
        /// The library is experimental
        /// </summary>
        Experimental = 2,

        /// <summary>
        /// WIP
        /// </summary>
        InDevelopment = 4,
        Alpha = 8,
        Release = 16,
    }
    /// <summary>
    /// Self-description of library type
    /// </summary>
    public enum StandardLibraryType
    {
        /// <summary>
        /// The library is official and part of core dev effort
        /// </summary>
        Core,
        /// <summary>
        /// The library is experimental
        /// </summary>
        Experimental,
        /// <summary>
        /// The library provides entry points or is a wrapper of other standard libraries
        /// </summary>
        MetaPackage,
        /// <summary>
        /// The library is a framework
        /// </summary>
        Framework
    }

    /// <summary>
    /// Base definition of standard libraries
    /// </summary>
    public abstract class StandardLibrary: CorePackage
    {
        /// <summary>
        /// Readiness pf package
        /// </summary>
        public abstract StandardLibraryStatus Status { get; }
        /// <summary>
        /// Provides retrivable self-identification of package type
        /// </summary>
        public abstract StandardLibraryType Type { get; }
        /// <summary>
        /// All standard libraries have the capability of issuing load-time instructions, but should generally be used only by meta-packages
        /// </summary>
        public virtual string? Instructions { get; } = null;
    }
}
