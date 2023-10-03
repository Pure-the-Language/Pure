using Core;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Notebook
{
    public static class Program
    {
        #region Main
        [STAThread]
        public static void Main(string[] args)
        {
            if (args.Length == 2 && args[0].ToLower() == "--create")
            {
                if (File.Exists(args[1]))
                    Console.WriteLine("File already exists.");
                else
                {
                    NotebookManager.CurrentNotebookFilePath = args[1];
                    NotebookManager.Save(new ApplicationData());
                }
                return;
            }

            var parent = ParentProcessUtilities.GetParentProcess();
            if (args.Length == 1 && args[0].ToLower() == "--debug")
            {
                Console.WriteLine(parent.ProcessName);
                return;
            }

            if (new string[] { "cwd", "powershell", "pwsh" }.Contains(parent.ProcessName.ToLower())) // Remar-cz: During debugging in VS it might be necessary to add "vsdebugconsole" to enter CLI mode
                BatchMode(args);
            else
            {
                var app = new App();
                app.InitializeComponent();
                app.Run();
            }
        }
        #endregion

        #region Batch Mode
        private static void BatchMode(string[] args)
        {
            if (args.Length == 0)
                Console.WriteLine("Missing file path.");
            else if (!File.Exists(args[0]))
                Console.WriteLine($"File doesn't exist: {args[0]}");
            else
            {
                string filepath = Path.GetFullPath(args[0]);
                Interpreter interpreter = new(null, filepath, args.Skip(1).ToArray(), null, null);
                
                ApplicationData data = LoadData(filepath);
                Directory.SetCurrentDirectory(Path.GetDirectoryName(filepath));
                ExecuteCells(interpreter, data, filepath);
            }

            static void ExecuteCells(Interpreter interpreter, ApplicationData data, string scriptPath)
            {
                foreach (CellBlock cell in data.Cells.Where(c => c.CellType == CellType.CSharp || c.CellType == CellType.Python))
                {
                    try
                    {
                        ExecuteCell(interpreter, cell, scriptPath);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error when executing cell ({cell.CellType}):");
                        Console.WriteLine(e.Message);
                        Console.WriteLine("Source: ");
                        Console.WriteLine(cell.Content);
                    }
                }
            }
            static void ExecuteCell(Interpreter interpreter, CellBlock cell, string scriptPath)
            {
                switch (cell.CellType)
                {
                    case CellType.CSharp:
                        foreach (var script in Interpreter.SplitScripts(cell.Content))
                            interpreter.Parse(script);
                        break;
                    case CellType.Python:
                        interpreter.Parse("Import(Python)");
                        interpreter.Parse($"""""
                                Python.Main.Parse("""
                                {cell.Content}
                                """);
                                """"");
                        break;
                    default:
                        throw new ArgumentException($"Invalid cell type: {cell.CellType}");
                }
            }
            static ApplicationData LoadData(string filepath)
            {
                string extension = Path.GetExtension(filepath);
                // Parse code blocks from MD files
                if (extension.ToLower() == ".md")
                    return MarkdownHelper.LoadDataFrom(filepath);
                else
                {

                    NotebookManager.CurrentNotebookFilePath = filepath;
                    return NotebookManager.Load();
                }
            }
        }
        #endregion
    }

    internal sealed class Win32
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        public static void HideConsoleWindow()
        {
            IntPtr hConsole = GetConsoleWindow();

            if (hConsole != IntPtr.Zero)
                ShowWindow(hConsole, 0); // 0 = SW_HIDE
        }
    }
}
