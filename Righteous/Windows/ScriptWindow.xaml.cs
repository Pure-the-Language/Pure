using ICSharpCode.AvalonEdit;
using Righteous.PopUps;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Righteous.Windows
{
    /// <summary>
    /// Interaction logic for ScriptWindow.xaml
    /// </summary>
    public partial class ScriptWindow : Window, INotifyPropertyChanged
    {
        #region Construction
        public ScriptWindow(DataModel model)
        {
            Model = model;
            InitializeComponent();
        }
        #endregion

        #region Data Binding Properties
        private DataModel _Model;
        public DataModel Model { get => _Model; set => SetField(ref _Model, value); }
        #endregion

        #region Events
        private void Window_LocationChanged(object sender, EventArgs e)
        {
            Model.Location = new Vector(Left, Top);
        }
        private void ExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            (App.Current.MainWindow as MainWindow).Evaluate(Model.Scripts);
        }
        private void ReorderButton_Click(object sender, RoutedEventArgs e)
        {
            string response = ListChoiceDialog.Prompt("Select Order", "Click \"Confirm\" to select new order for the step", Model.ID.ToString(), (App.Current.MainWindow as MainWindow).Data.Steps.Select(s => s.ID.ToString()).ToArray(), null, "Confirm");
            if (response != null)
                (App.Current.MainWindow as MainWindow).Data.Reorder(Model, int.Parse(response));
        }
        #endregion

        #region Commands
        private void ExecuteCommand_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
            => e.CanExecute = true;
        private void ExecuteCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            Model.Scripts = AvalonEdit.Text;
            (App.Current.MainWindow as MainWindow).Evaluate(Model.Scripts);
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

        #region Text Editing
        private void AvalonTextEditor_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            TextEditor editor = sender as TextEditor;
            editor.Text = Model.Scripts;
        }
        private void AvalonTextEditor_Initialized(object sender, EventArgs e)
        {
            TextEditor editor = sender as TextEditor;
            editor.Text = Model.Scripts;
        }
        private void AvalonTextEditor_OnTextChanged(object sender, EventArgs e)
        {
            TextEditor editor = sender as TextEditor;
            Model.Scripts = editor.Text;
        }
        #endregion
    }
}
