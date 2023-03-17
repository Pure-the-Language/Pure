using Core;
using ICSharpCode.AvalonEdit;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Win32;
using NuGet.Protocol.Plugins;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace Notebook
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Construction
        public MainWindow()
        {
            InitializeComponent();

            Interpreter = new Interpreter();
            Interpreter.Start(OutputHandler, """
                    Pure v0.0.1
                    """);
        }
        private Interpreter Interpreter;
        private void OutputHandler(string message)
        {
            if (CurrentCell != null && Data.Cells.Contains(CurrentCell))
                GenerateOutputCell(CurrentCell, message, false);
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
        #endregion

        #region Menu Items
        private void OpenFileMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
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
                NotebookManager.CurrentNotebookFilePath = saveFileDialog.FileName;
                Title = $"Pure - {saveFileDialog.FileName}";
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
            throw new NotImplementedException();
        }
        private void ExecuteCellMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentCell != null && Data.Cells.Contains(CurrentCell))
                ExecuteCell(CurrentCell);
        }
        private void ExecuteAllMenuItem_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
        private void ExportMarkdownMenuItem_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
        private void ExportCodeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
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
        private void ExecuteAllCellsCommand_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
            => e.CanExecute = true;
        private void ExecuteAllCellsCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
            => ExecuteAllMenuItem_Click(null, null);
        #endregion

        #region Routines
        private void AddCell(CellBlock newCell)
        {
            if (CurrentCell == null)
                Data.Cells.Insert(0, newCell);
            else
            {
                int cellIndex = Data.Cells.IndexOf(CurrentCell);
                if (Data.Cells.Count > cellIndex + 1 && Data.Cells[cellIndex + 1].CellType == CellType.CacheOutput)
                    cellIndex = cellIndex + 1;
                Data.Cells.Insert(cellIndex + 1, newCell);
            }
            CurrentCell = newCell;
        }
        private void GenerateOutputCell(CellBlock codeCell, string message, bool reInitialize)
        {
            int cellIndex = Data.Cells.IndexOf(codeCell);
            CellBlock outputCell = null;
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
        private void ExecuteCell(CellBlock cell)
        {
            if (cell.CellType == CellType.CacheOutput || cell.CellType == CellType.Markdown)
                throw new InvalidOperationException($"Invalid cell type: {cell.CellType}");

            GenerateOutputCell(cell, string.Empty, true);
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
    }
}
