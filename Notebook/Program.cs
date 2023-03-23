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
            var parent = ParentProcessUtilities.GetParentProcess();
            if (args.Length == 1 && args[0].ToLower() == "--debug")
            {
                Console.WriteLine(parent.ProcessName);
                return;
            }

            if (new string[] { "cwd", "powershell", "pwsh" }.Contains(parent.ProcessName.ToLower()))
            {
                BatchMode(args);
            }
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
                Interpreter interpreter = new Interpreter();
                interpreter.Start(null, null, null, args.Skip(1).ToArray());

                NotebookManager.CurrentNotebookFilePath = filepath;
                var data = NotebookManager.Load();
                Directory.SetCurrentDirectory(Path.GetDirectoryName(filepath));
                
                ExecuteCells(interpreter, data);
            }

            static void ExecuteCells(Interpreter interpreter, ApplicationData data)
            {
                foreach (CellBlock cell in data.Cells.Where(c => c.CellType == CellType.CSharp || c.CellType == CellType.Python))
                {
                    try
                    {
                        ExecuteCell(interpreter, cell);
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
            static void ExecuteCell(Interpreter interpreter, CellBlock cell)
            {
                switch (cell.CellType)
                {
                    case CellType.CSharp:
                        foreach (var script in Parser.SplitScripts(cell.Content))
                            interpreter.Evaluate(script);
                        break;
                    case CellType.Python:
                        interpreter.Evaluate("Import(Python)");
                        interpreter.Evaluate($"""""
                                Evaluate("""
                                {cell.Content}
                                """);
                                """"");
                        break;
                    default:
                        throw new ArgumentException($"Invalid cell type: {cell.CellType}");
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
