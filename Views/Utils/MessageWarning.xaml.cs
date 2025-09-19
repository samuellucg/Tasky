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
    /// Lógica interna para MessageWarning.xaml
    /// </summary>
    public partial class MessageWarning : Window
    {
        public MessageWarning(string message = "Tarefa criada com sucesso!")
        {
            this.Owner = Application.Current.MainWindow;
            InitializeComponent();
            MessageToUser.Text = message;
            if(!IsActive)
                ShowDialog();
        }

    }
}
