using System.Windows;

namespace Notebook
{
    public partial class EntryWindow : Window
    {
        public EntryWindow(string text)
        {
            InitializeComponent();
            EntryText.Text = text;
        }
        public string Result { get; set; }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Result = EntryText.Text;
            DialogResult = true;
        }
    }
}
