namespace Core
{
    public static class Construct
    {
        #region Helper Construct
        public static void Help(object instance)
        {
            var type = instance.GetType();
            RoslynContext.PrintType(type);
        }
        #endregion

        #region Configurations
        private static string DefaultNugetCacheStorePath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Pure", "Nugets");
        public static string NugetCacheStore
        {
            get
            {
                string path = DefaultNugetCacheStorePath;
                Directory.CreateDirectory(path);
                return path;
            }
        }
        #endregion
    }
}
