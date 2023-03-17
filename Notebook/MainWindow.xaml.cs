using Microsoft.Win32;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Notebook
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Construction
        public MainWindow()
        {
            InitializeComponent();
        }
        #endregion

        #region Data Binding Properties
        private ApplicationData _Data = NotebookManager.Load();
        public ApplicationData Data { get => _Data; set => SetField(ref _Data, value); }
        #endregion

        #region Windows Events

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

        }
        private void ExecuteAllMenuItem_Click(object sender, RoutedEventArgs e)
        {

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
