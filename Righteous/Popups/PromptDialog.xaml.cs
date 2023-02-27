using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Righteous.PopUps
{
    public partial class PromptDialog : Window
    {
        public enum InputType
        {
            Text,
            Password
        }

        #region Construction
        public PromptDialog(string question, string title, string defaultValue = "", InputType inputType = InputType.Text, string furtherExplanation = null, string alternativeOkButtonLabel = null, string alternativeCancelButtonLabel = null)
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(PromptDialogLoaded);
            QuestionText.Text = question;
            Title = title;
            GeneralTextResponse.Text = defaultValue;

            _InputType = inputType;
            if (_InputType == InputType.Password)
                GeneralTextResponse.Visibility = Visibility.Collapsed;
            else
                PasswordTextResponse.Visibility = Visibility.Collapsed;

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
        private readonly InputType _InputType = InputType.Text;
        public string ResponseText
        {
            get
            {
                if (_InputType == InputType.Password)
                    return PasswordTextResponse.Password;
                else
                    return GeneralTextResponse.Text;
            }
        }
        #endregion

        #region Events
        void PromptDialogLoaded(object sender, RoutedEventArgs e)
        {
            if (_InputType == InputType.Password)
                PasswordTextResponse.Focus();
            else
                GeneralTextResponse.Focus();
        }
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CancelBUtton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion

        #region Interface Method
        public static string Prompt(string question, string title, string defaultValue = "", InputType inputType = InputType.Text, string furtherExplanation = null, string alternativeOkButtonLabel = null, string alternativeCancelButtonLabel = null)
        {
            PromptDialog inst = new(question, title, defaultValue, inputType, furtherExplanation, alternativeOkButtonLabel, alternativeCancelButtonLabel);
            inst.ShowDialog();
            if (inst.DialogResult == true)
                return inst.ResponseText;
            return null;
        }
        #endregion     
    }
}
