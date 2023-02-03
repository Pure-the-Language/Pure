using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public static class Construct
    {
        #region Main Construct
        public static Assembly Import(string dllName, bool importNamespaces = true)
        {
            string filePath = dllName;
            if (!File.Exists(dllName))
                filePath = TryFindDLLFile(dllName);

            if (filePath != null && File.Exists(filePath))
            {
                var assembly = Assembly.LoadFrom(filePath); // Might load from within the Roslyn state's context//app domain?
                RoslynContext.Singleton.AddReference(assembly);

                if (importNamespaces)
                    foreach (var ns in assembly.GetTypes().Where(t => t.IsVisible)
                            .Select(t => t.Namespace).Distinct())
                        RoslynContext.Singleton.AddImport(ns);
                return assembly;
            }
            else throw new ArgumentException($"Cannot find package: {dllName}");

            static string TryFindDLLFile(string dllName)
            {
                foreach (var path in Environment.GetEnvironmentVariable("PATH")
                .Split(';', StringSplitOptions.RemoveEmptyEntries))
                {
                    try
                    {
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
        public static void Include(string scriptName)
        {
            Console.WriteLine("Script");
        }
        #endregion
    }
}
