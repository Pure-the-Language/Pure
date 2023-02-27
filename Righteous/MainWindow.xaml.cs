using Core;
using Microsoft.Win32;
using Righteous.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Righteous
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
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
        #endregion

        #region Data Binding Properties
        private string _DisplayText;
        public string DisplayText { get => _DisplayText; set => SetField(ref _DisplayText, value); }
        private string _CurrentFilePath;
        public string CurrentFilePath { get => _CurrentFilePath; 
            set
            {
                SetField(ref _CurrentFilePath, value);
                Interpreter = new Interpreter();
                Interpreter.Start(OutputHandler, """
                    Pure v0.0.1
                    """);
            }
        }
        private ApplicationData _Data = new ();
        public ApplicationData Data { get => _Data; set => SetField(ref _Data, value); }
        #endregion

        #region Commands
        private void FileNewCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
            => e.CanExecute = true;
        private void FileNewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
            => CreateNewFile();
        private void FileOpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
            => OpenFile();
        private void FileOpenCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
            => e.CanExecute = true;
        private void FileSaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
            => SaveFile();
        private void FileSaveCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
            => e.CanExecute = true;
        #endregion

        #region Methods
        internal void Evaluate(string scripts)
        {
            if (scripts.Contains(';'))
                Interpreter.Evaluate(scripts);
            else
            {
                // Intepret only lines
                foreach (var parts in scripts.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
                    Interpreter.Evaluate(parts);
            }
        }
        #endregion

        #region Menu Items
        private void NewFileMenuItem_Click(object sender, RoutedEventArgs e)
            => CreateNewFile();
        private void OpenFileMenuItem_Click(object sender, RoutedEventArgs e)
            => OpenFile();

        private void SaveFileAsMenuItem_Click(object sender, RoutedEventArgs e)
            => SaveFile(true);

        private void SaveFileMenuItem_Click(object sender, RoutedEventArgs e)
            => SaveFile();
        #endregion

        #region Events
        private void NewStepButton_Click(object sender, RoutedEventArgs e)
        {
            DataModel model = new DataModel(Data.Steps.Count)
            {
                Name = $"Step {Data.Steps.Count}"
            };
            Data.Steps.Add(model);
            new ScriptWindow(model)
            {
                Owner = this
            }.Show();
        }

        private void ShowAllStepsButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window != this)
                    window.Close();
            }
            foreach (var step in Data.Steps)
            {
                new ScriptWindow(step)
                {
                    Owner = this,
                    WindowStartupLocation = WindowStartupLocation.Manual,
                    Left = step.Location.X,
                    Top = step.Location.Y
                }.Show();
            }
        }
        private void CloseAllStepsButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window != this)
                    window.Close();
            }
        }
        private void RunAllStepsButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (DataModel step in Data.Steps)
                Evaluate(step.Scripts);
        }
        #endregion

        #region File Actions
        private void CreateNewFile()
        {
            SaveFileDialog saveFileDialog = new()
            {
                Filter = "Righteous (*.prt)|*.prt|All (*.*)|*.*",
                AddExtension = true,
                Title = "Choose location to save file"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                CurrentFilePath = saveFileDialog.FileName;
                Data = new ApplicationData()
                {
                    Name = "New Analysis"
                };
                ApplicationDataSerializer.Save(CurrentFilePath, Data);
                Title = $"Righteous - {CurrentFilePath}";
            }
        }
        private void OpenFile()
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = "Righteous (*.prt)|*.prt|All (*.*)|*.*",
                Title = "Choose file to open"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                CurrentFilePath = openFileDialog.FileName;
                Data = OpenFile(CurrentFilePath);
                Title = $"Righteous - {CurrentFilePath}";
            }
        }
        private void SaveFile(bool saveNewFile = false)
        {
            if (saveNewFile || CurrentFilePath == null)
            {
                SaveFileDialog saveFileDialog = new()
                {
                    Filter = "Righteous (*.prt)|*.prt|All (*.*)|*.*",
                    AddExtension = true,
                    Title = "Choose location to save file as"
                };
                if (saveFileDialog.ShowDialog() == true)
                {
                    CurrentFilePath = saveFileDialog.FileName;
                    ApplicationDataSerializer.Save(CurrentFilePath, Data);
                    Title = $"Righteous - {CurrentFilePath}";
                }
            }
            else
                ApplicationDataSerializer.Save(CurrentFilePath, Data);
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

        #region Routines
        private static ApplicationData OpenFile(string filePath)
        {
            var appData = ApplicationDataSerializer.Load(filePath);
            return appData;
        }
        private void OutputHandler(string message)
        {
            DisplayText = message;
        }
        #endregion
    }
}
