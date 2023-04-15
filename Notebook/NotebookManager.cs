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
                string path = Path.Combine(DefaultNotebookFolder, "Default.pnb");
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
        internal static void Save(ApplicationData data, bool asBackup = false)
        {
            string savePath = NotebookFilePath + (asBackup ? ".backup" : string.Empty);
            ApplicationData.Save(savePath, data);

            // In case no changes are made, don't create auto save file
            // Remark-cz: For slightly faster implementation, we can compare directly in-memory streams instead of actually writing to file first
            if (asBackup && File.Exists(NotebookFilePath) && CompareFileEquals(NotebookFilePath, savePath))
                File.Delete(savePath);
        }
        #endregion

        #region Helpers
        public static bool CompareFileEquals(string path1, string path2)
        {
            byte[] file1 = System.IO.File.ReadAllBytes(path1);
            byte[] file2 = System.IO.File.ReadAllBytes(path2);
            if (file1.Length == file2.Length)
            {
                for (int i = 0; i < file1.Length; i++)
                    if (file1[i] != file2[i])
                        return false;
                return true;
            }
            return false;
        }
        #endregion
    }
}
