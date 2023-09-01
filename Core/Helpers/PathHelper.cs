namespace Core.Helpers
{
    public static class PathHelper
    {
        public static string FindScriptFileFromEnvPath(string scriptName, string currentScriptFile = null)
            => FindFileFromEnvironmentVariablePaths("PUREPATH", scriptName, true,
                currentScriptFile != null ? new[] { Path.GetDirectoryName(Path.GetFullPath(currentScriptFile)) } : null, new[] { ".cs" });
        public static string FindDLLFileFromEnvPath(string dllName)
            => FindFileFromEnvironmentVariablePaths(variableName: "PATH", targetName: dllName, searchNameAsFolder: true, 
                additionalSearchFolders: null, additionalExtensions: new string[] { ".dll", ".exe" });
        public static string FindFileFromEnvironmentVariablePaths(string variableName, string targetName, 
            bool searchNameAsFolder,
            string[] additionalSearchFolders, string[] additionalExtensions)
        {
            additionalSearchFolders ??= Array.Empty<string>();
            additionalExtensions ??= Array.Empty<string>();

            // Try use full path
            string fullpath = Path.GetFullPath(targetName);
            if (File.Exists(fullpath))
                return fullpath;
            // Try using additional search folders
            foreach (var additionalFolder in additionalSearchFolders)
            {
                fullpath = Path.Combine(additionalFolder, targetName);
                if (File.Exists(fullpath))
                    return fullpath;
                foreach (var suffix in additionalExtensions)
                    if (File.Exists(fullpath + suffix))
                        return fullpath + suffix;
            }
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(variableName)))
                return null;
            // Try using env variable containing paths
            foreach (string path in Environment.GetEnvironmentVariable(variableName).SplitArgumentsLikeCsv(';', true))
            {
                if (!Directory.Exists(path)) continue;
                try
                {
                    // Use name as folder and search inside for the file with expected extension and the same name as folder name
                    if (searchNameAsFolder && additionalExtensions.All(et => !targetName.EndsWith(et))
                        && Directory.Exists(Path.Combine(path, targetName)))
                    {
                        string[] actualFilePaths = additionalExtensions.Select(et => Path.Combine(path, targetName, targetName + et)).ToArray();
                        string found = actualFilePaths.FirstOrDefault(File.Exists);
                        if (found != null)
                            return found;
                    }

                    // Search from environment path
                    fullpath = Path.Combine(path, targetName);
                    if (File.Exists(fullpath))
                        return fullpath;
                    foreach (string file in Directory.EnumerateFiles(path))
                    {
                        string fileName = Path.GetFileName(file);
                        string fileNameNoExtention = Path.GetFileNameWithoutExtension(file);
                        string extension = Path.GetExtension(file).ToLower();
                        if (additionalExtensions.Contains(extension))
                        {
                            if (fileName == targetName || fileNameNoExtention == targetName)
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
