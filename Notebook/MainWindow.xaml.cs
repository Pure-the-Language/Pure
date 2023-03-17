using Core;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Win32;
using NuGet.Protocol.Plugins;
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
            {
                int cellIndex = Data.Cells.IndexOf(CurrentCell);
                if (Data.Cells.Count > cellIndex + 1 && Data.Cells[cellIndex + 1].CellType == CellType.CacheOutput)
                    Data.Cells[cellIndex + 1].Content += message;
                else
                    Data.Cells.Insert(cellIndex + 1, new CellBlock()
                    {
                        CellType = CellType.CacheOutput,
                        Content = message
                    });
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
        #endregion

        #region Menu Items
        private void OpenFileMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Title = "Select path to save file",
                Filter = "Pure Notebook Files (*.md)|*.md|All (*.*)|*.*",
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
        private void AddCellMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Data.Cells.Add(new CellBlock()
            {
                CellType = CellType.Code
            });
        }
        private void DeleteCellMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }
        private void ExecuteCellMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentCell != null && Data.Cells.Contains(CurrentCell))
            {
                int cellIndex = Data.Cells.IndexOf(CurrentCell);
                if (Data.Cells.Count > cellIndex + 1 && Data.Cells[cellIndex + 1].CellType == CellType.CacheOutput)
                    Data.Cells[cellIndex + 1].Content = string.Empty;
                else
                    Data.Cells.Insert(cellIndex + 1, new CellBlock()
                    {
                        CellType = CellType.CacheOutput,
                        Content = string.Empty
                    });

                Interpreter.Evaluate(CurrentCell.Content);
            }
        }
        private void ExecuteAllMenuItem_Click(object sender, RoutedEventArgs e)
        {

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
        private void CreateCellCommand_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
            => e.CanExecute = true;
        private void CreateCellCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
            => AddCellMenuItem_Click(null, null);
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
