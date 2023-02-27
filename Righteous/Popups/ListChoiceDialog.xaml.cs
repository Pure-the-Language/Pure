using System.Windows;
using System.Windows.Input;

namespace Righteous.PopUps
{
    public partial class ListChoiceDialog : Window
    {
        #region Construction
        public ListChoiceDialog(string title, string text, string defaultValue, string[] options, string furtherExplanation = null, string alternativeOkButtonLabel = null, string alternativeCancelButtonLabel = null)
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(ListChoiceDialogLoaded);

            DisplayText.Text = text;
            Title = title;

            foreach (var option in options)
                OptionsListBox.Items.Add(option);
            OptionsListBox.SelectedValue = defaultValue;

            if (!string.IsNullOrEmpty(furtherExplanation))
            {
                ExplanationText.Text = furtherExplanation;
                ExplanationText.Visibility = Visibility.Visible;
            }

            if (!string.IsNullOrEmpty(alternativeOkButtonLabel))
                OkButton.Content = "_" + alternativeOkButtonLabel;
            if (!string.IsNullOrEmpty(alternativeCancelButtonLabel))
                CancelButton.Content = "_" + alternativeCancelButtonLabel;
        }
        #endregion

        #region Properties
        public string ResponseText => OptionsListBox.SelectedValue as string;
        #endregion

        #region Events
        void ListChoiceDialogLoaded(object sender, RoutedEventArgs e)
        {
            OptionsListBox.Focus();
        }
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CancelBUtton_Click(object sender, RoutedEventArgs e)
            => Close();
        #endregion

        #region Interface Method
        public static string Prompt(string title, string text, string defaultValue, string[] options, string furtherExplanation = null, string alternativeOkButtonLabel = null, string alternativeCancelButtonLabel = null)
        {
            ListChoiceDialog inst = new(title, text, defaultValue, options, furtherExplanation, alternativeOkButtonLabel, alternativeCancelButtonLabel);
            inst.ShowDialog();
            if (inst.DialogResult == true)
                return inst.ResponseText;
            return null;
        }
        #endregion     
    }
}
