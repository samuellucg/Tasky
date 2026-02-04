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

namespace Tasky.Views.Utils
{
    /// <summary>
    /// Lógica interna para MessageConfirmation.xaml
    /// </summary>
    public partial class MessageConfirmation : Window
    {
        #region Attributes
        public bool UserResponse { get; private set; }
        #endregion

        #region Constructor
        public MessageConfirmation(string confirmationText, bool showAttention = true)
        {
            this.Owner = Application.Current.MainWindow;
            InitializeComponent();
            ConfirmationToUser.Text = confirmationText;
            attentionName.Visibility = showAttention == true ? Visibility.Visible : Visibility.Collapsed;
            submitButton.Visibility = showAttention == true ? Visibility.Visible : Visibility.Collapsed;
            cancelButton.Visibility = showAttention == true ? Visibility.Visible : Visibility.Collapsed;
        }
        #endregion

        #region Functions
        private void SubmitConfirmation(object sender, RoutedEventArgs e)
        {
            UserResponse = true;
            this.DialogResult = true;
        } 
        private void UndoConfirmation(object sender, RoutedEventArgs e)
        {
            UserResponse = false;
            this.DialogResult = true;
        }
        #endregion
    }
}
