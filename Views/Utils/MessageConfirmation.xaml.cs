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
        public bool UserResponse { get; private set; }

        public MessageConfirmation(string confirmationText)
        {
            this.Owner = Application.Current.MainWindow;
            InitializeComponent();
            ConfirmationToUser.Text = confirmationText;
        }

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
    }
}
