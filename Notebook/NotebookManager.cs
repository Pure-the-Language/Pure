using System;
using System.IO;
using System.Linq;

namespace Notebook
{
    public static class NotebookManager
    {
        #region Private
        private static string DefaultNotebookFolder => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Pure Notebooks");
        private static string DefaultNotebookFilePath
        {
            get
            {
                Directory.CreateDirectory(DefaultNotebookFolder);
                string path = Path.Combine(DefaultNotebookFolder, "Default.md");
                return path;
            }
        }
        private static string NotebookFilePath
        {
            get
            {
                string path = CurrentNotebookFilePath ?? DefaultNotebookFilePath;
                if (!File.Exists(path))
                    ApplicationData.Save(path, new ApplicationData());
                return path;
            }
        }
        #endregion

        #region Interface
        public static string CurrentNotebookFilePath { private get; set; }
        public static ApplicationData Load()
        {
            return ApplicationData.Load(NotebookFilePath);
        }
        internal static void Save(ApplicationData data)
        {
            ApplicationData.Save(NotebookFilePath, data);
        }
        #endregion
    }
}
