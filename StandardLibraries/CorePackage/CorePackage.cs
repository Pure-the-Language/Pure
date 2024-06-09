namespace CorePackage
{
    /// <summary>
    /// Base abstraction for Pure libraries and provides essential information.
    /// </summary>
    public abstract class CorePackage
    {
        /// <summary>
        /// Provides name of package, should be the same as assembly file name
        /// </summary>
        public abstract string Name { get; }
        /// <summary>
        /// Provides version of package, should be the same as assembly file version
        /// </summary>
        public abstract string Version { get; }
        /// <summary>
        /// Provides description of package
        /// </summary>
        public abstract string Description { get; }
        /// <summary>
        /// Provides textual description of package dependancies, should be the same as assembly project file dependancies and as shown in Nuget
        /// </summary>
        public abstract string Dependancies { get; }
    }
}
