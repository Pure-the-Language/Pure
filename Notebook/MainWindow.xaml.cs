using Core;
using ICSharpCode.AvalonEdit;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Notebook
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Construction
        public MainWindow()
        {
            // Hide console
            Win32.HideConsoleWindow();

            InitializeComponent();

            var args = Environment.GetCommandLineArgs();
            Interpreter = new Interpreter();
            Interpreter.Start(OutputHandlerNonMainThread, """
                    Pure v0.0.1
                    """, null, args.Length > 2 ? args.Skip(2).ToArray() : null);

            if (args.Length >= 2)
            {
                string filepath = args[1];
                if (File.Exists(filepath))
                    OpenFile(filepath);
            }
        }
        private readonly Dispatcher MainUIDispatcher = Dispatcher.CurrentDispatcher;
        private readonly Interpreter Interpreter;
        private void OutputHandlerNonMainThread(string message)
        {
            MainUIDispatcher.Invoke(OutputHandler);

            void OutputHandler()
            {
                if (CurrentCell != null && Data.Cells.Contains(CurrentCell))
                    GenerateOutputCell(CurrentCell, message, false);
            }
        }
        #endregion

        #region Data Binding Properties
        private CellBlock _CurrentCell = null;
        private ApplicationData _Data = NotebookManager.Load();

        public CellBlock CurrentCell { get => _CurrentCell; set => SetField(ref _CurrentCell, value); }
        public ApplicationData Data { get => _Data; set => SetField(ref _Data, value); }
        #endregion

        #region Windows Events
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            Control control = sender as Control;
            CellBlock cellBlock = control.DataContext as CellBlock;
            CurrentCell = cellBlock;
        }
        private void AvalonTextEditor_GotFocus(object sender, RoutedEventArgs e)
        {
            TextEditor editor = sender as TextEditor;
            CellBlock cellBlock = editor.DataContext as CellBlock;
            CurrentCell = cellBlock;
        }
        private void AvalonTextEditor_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            TextEditor editor = sender as TextEditor;
            CellBlock block = editor.DataContext as CellBlock;
            editor.Text = block.Content;
        }
        private void AvalonTextEditor_Initialized(object sender, System.EventArgs e)
        {
            TextEditor editor = sender as TextEditor;
            CellBlock block = editor.DataContext as CellBlock;
            editor.Text = block.Content;
        }
        private void AvalonTextEditor_OnTextChanged(object sender, System.EventArgs e)
        {
            TextEditor editor = sender as TextEditor;
            CellBlock block = editor.DataContext as CellBlock;
            block.Content = editor.Text;
        }
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            NotebookManager.Save(Data, true);
        }
        #endregion

        #region Menu Items
        private void OpenFileMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new()
            {
                Title = "Select path to save file",
                Filter = "Pure Notebook Files (*.pnb)|*.pnb|All (*.*)|*.*",
                CheckFileExists = false,
                OverwritePrompt = false,
                CreatePrompt = false,
                CheckPathExists = true,
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                string filepath = saveFileDialog.FileName;
                OpenFile(filepath);
            }
            e.Handled = true;
        }
        private void SaveFileMenuItem_Click(object sender, RoutedEventArgs e)
        {
            NotebookManager.Save(Data);
        }
        private void AddMarkdownCellMenuItem_Click(object sender, RoutedEventArgs e)
        {
            AddCell(new CellBlock()
            {
                CellType = CellType.Markdown
            });
        }
        private void AddCSharpCellMenuItem_Click(object sender, RoutedEventArgs e)
        {
            AddCell(new CellBlock()
            {
                CellType = CellType.CSharp
            });
        }
        private void AddPythonCellMenuItem_Click(object sender, RoutedEventArgs e)
        {
            AddCell(new CellBlock()
            {
                CellType = CellType.Python
            });
        }
        private void DeleteCellMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentCell != null && Data.Cells.Contains(CurrentCell))
                DeleteCell(CurrentCell);
        }
        private void ExecuteCellMenuItem_Click(object sender, RoutedEventArgs e)
        {
            NotebookManager.Save(Data, true);
            if (CurrentCell != null && Data.Cells.Contains(CurrentCell)
                && CurrentCell.CellType != CellType.Markdown
                && CurrentCell.CellType != CellType.CacheOutput)
                ExecuteCell(CurrentCell);
        }
        private void ExecuteAllMenuItem_Click(object sender, RoutedEventArgs e)
        {
            NotebookManager.Save(Data, true);
            Data.Cells = new (Data.Cells.Where(c => c.CellType != CellType.CacheOutput));
            foreach (var cell in Data.Cells.ToArray()
                .Where(c => c.CellType == CellType.CSharp || c.CellType == CellType.Python))
                ExecuteCell(cell);
        }
        private void ExportMarkdownMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new()
            {
                Title = "Select path to save file",
                FileName = "Export",
                Filter = "Markdown (*.md)|*.md|All (*.*)|*.*"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                string filepath = saveFileDialog.FileName;
                File.WriteAllText(filepath, string.Join("\n\n", Data.Cells
                    .Where(c => c.CellType != CellType.CacheOutput)
                    .Select(c => c.CellType == CellType.Markdown 
                        ? c.Content 
                        : $"""
                        ```{(c.CellType == CellType.Python ? "Python" : "C#")}
                        {c.Content}
                        ```
                        """)));
            }
            e.Handled = true;
        }
        private void ExportCodeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new()
            {
                Title = "Select path to save file",
                FileName = "Export",
                Filter = "C# (*.cs)|*.cs|Python (*.py)|*.py"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                string filepath = saveFileDialog.FileName;
                string extension = Path.GetExtension(filepath);
                if (extension == ".cs")
                    File.WriteAllText(filepath, string.Join("\n", Data.Cells.Where(c => c.CellType == CellType.CSharp).Select(c => c.Content)));
                else if (extension == ".py")
                    File.WriteAllText(filepath, string.Join("\n", Data.Cells.Where(c => c.CellType == CellType.Python).Select(c => c.Content)));
            }
            e.Handled = true;
        }
        #endregion

        #region Keyboard Commands
        private void FileOpenCommand_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
            => e.CanExecute = true;
        private void FileOpenCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
            => OpenFileMenuItem_Click(null, null);
        private void FileSaveCommand_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
            => e.CanExecute = true;
        private void FileSaveCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
            => SaveFileMenuItem_Click(null, null);
        private void CreateCSharpCellCommand_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
            => e.CanExecute = true;
        private void CreateCSharpCellCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
            => AddCSharpCellMenuItem_Click(null, null);
        private void CreatePythonCellCommand_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
            => e.CanExecute = true;
        private void CreatePythonCellCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
            => AddPythonCellMenuItem_Click(null, null);
        private void CreateMarkdownCellCommand_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
            => e.CanExecute = true;
        private void CreateMarkdownCellCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
            => AddMarkdownCellMenuItem_Click(null, null);
        private void DeleteCellCommand_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
            => e.CanExecute = true;
        private void DeleteCellCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
            => DeleteCellMenuItem_Click(null, null);
        private void ExecuteCellCommand_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
            => e.CanExecute = true;
        private void ExecuteCellCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
            => ExecuteCellMenuItem_Click(null, null);
        #endregion

        #region Routines
        private void OpenFile(string filepath)
        {
            NotebookManager.CurrentNotebookFilePath = filepath;
            Title = $"Pure - {filepath}";
            Data = NotebookManager.Load();
            Directory.SetCurrentDirectory(Path.GetDirectoryName(filepath));
        }
        private void AddCell(CellBlock newCell)
        {
            if (CurrentCell == null)
                Data.Cells.Insert(0, newCell);
            else
            {
                int cellIndex = Data.Cells.IndexOf(CurrentCell);
                if (Data.Cells.Count > cellIndex + 1 && Data.Cells[cellIndex + 1].CellType == CellType.CacheOutput)
                    cellIndex++;
                Data.Cells.Insert(cellIndex + 1, newCell);
            }
            CurrentCell = newCell;

            // Auto-focus
            Task.Delay(300).ContinueWith(t =>
            {
                Dispatcher.Invoke(() =>
                {
                    if (CurrentCell.CellType == CellType.Python || CurrentCell.CellType == CellType.CSharp)
                    {
                        var te = FindVisualChildren<TextEditor>(DataItemsControl).SingleOrDefault(te => (te.DataContext as CellBlock) == CurrentCell);
                        te.Focus();
                    }
                });
            });
        }
        private void GenerateOutputCell(CellBlock codeCell, string message, bool reInitialize)
        {
            int cellIndex = Data.Cells.IndexOf(codeCell);
            CellBlock outputCell;
            if (Data.Cells.Count > cellIndex + 1 && Data.Cells[cellIndex + 1].CellType == CellType.CacheOutput)
                outputCell = Data.Cells[cellIndex + 1];
            else
            {
                outputCell = new CellBlock()
                {
                    CellType = CellType.CacheOutput,
                    Content = string.Empty
                };
                Data.Cells.Insert(cellIndex + 1, outputCell);
            }

            if (reInitialize)
                outputCell.Content = message;
            else
                outputCell.Content += message;
        }
        private void DeleteCell(CellBlock cell)
        {
            int cellIndex = Data.Cells.IndexOf(cell);
            if (cell.CellType != CellType.CacheOutput && Data.Cells.Count > cellIndex + 1 && Data.Cells[cellIndex + 1].CellType == CellType.CacheOutput)
            {
                CellBlock outputCell = Data.Cells[cellIndex + 1];
                Data.Cells.Remove(outputCell);
            }
            Data.Cells.Remove(cell);
        }
        private void ExecuteCell(CellBlock cell)
        {
            Task.Run(ExecuteCellThreaded);

            void ExecuteCellThreaded()
            {
                if (cell.CellType == CellType.CacheOutput || cell.CellType == CellType.Markdown)
                    throw new InvalidOperationException($"Invalid cell type: {cell.CellType}");

                Dispatcher.Invoke(() => GenerateOutputCell(cell, string.Empty, true));
                switch (cell.CellType)
                {
                    case CellType.CSharp:
                        foreach (var script in Parser.SplitScripts(cell.Content))
                            Interpreter.Evaluate(script);
                        break;
                    case CellType.Python:
                        Interpreter.Evaluate("Import(Python)");
                        Interpreter.Evaluate($"""""
                        Evaluate("""
                        {cell.Content}
                        """);
                        """"");
                        break;
                    case CellType.Markdown:
                    case CellType.CacheOutput:
                    default:
                        throw new ArgumentException($"Invalid cell type: {cell.CellType}");
                }
            }
        }
        #endregion

        #region Data Binding
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        protected bool SetField<TType>(ref TType field, TType value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<TType>.Default.Equals(field, value)) return false;
            field = value;
            NotifyPropertyChanged(propertyName);
            return true;
        }
        #endregion

        #region Helpers
        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) yield return (T)Enumerable.Empty<T>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject ithChild = VisualTreeHelper.GetChild(depObj, i);
                if (ithChild == null) continue;
                if (ithChild is T t) yield return t;
                foreach (T childOfChild in FindVisualChildren<T>(ithChild)) yield return childOfChild;
            }
        }
        #endregion
    }
}
