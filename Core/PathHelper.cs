namespace Core
{
    public static class PathHelper
    {
        public static string FindDLLFileFromEnvPath(string dllName)
        {
            // Try use full path
            string fullpath = Path.GetFullPath(dllName);
            if (File.Exists(fullpath))
                return fullpath;
            // Try using PATH env variable
            foreach (var path in Environment.GetEnvironmentVariable("PATH").SplitArgumentsLikeCsv(';', true))
            {
                if (!Directory.Exists(path)) continue;
                try
                {
                    if (!dllName.EndsWith(".dll") && !dllName.EndsWith(".exe")
                        && Directory.Exists(Path.Combine(path, dllName)))
                    {
                        string dllPath = Path.Combine(path, dllName, dllName);
                        if (File.Exists(dllPath + ".exe"))
                            return dllPath + ".exe";
                        if (File.Exists(dllPath + ".dll"))
                            return dllPath + ".dll";
                    }
                    fullpath = Path.Combine(path, dllName);
                    if (File.Exists(fullpath))
                        return fullpath;
                    foreach (var file in Directory.EnumerateFiles(path))
                    {
                        string fileName = Path.GetFileName(file);
                        string fileNameNoExtention = Path.GetFileNameWithoutExtension(file);
                        string extension = Path.GetExtension(file).ToLower();
                        if (extension == ".dll" || extension == ".exe")
                        {
                            if (fileName == dllName || fileNameNoExtention == dllName)
                                return file;
                        }
                    }
                }
                // Remark-cz: Certain paths might NOT be enumerable due to access issues
                catch (Exception) { continue; }
            }
            return null;
        }
    }
}
