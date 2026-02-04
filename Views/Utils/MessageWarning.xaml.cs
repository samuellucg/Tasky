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
using NLog;

namespace Tasky.Views.Utils
{
    /// <summary>
    /// Lógica interna para MessageWarning.xaml
    /// </summary>
    public partial class MessageWarning : Window
    {
        #region Attributes
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        #endregion

        #region Constructor
        public MessageWarning(string message = "Tarefa criada com sucesso!")
        {
            try
            {

                this.Owner = Application.Current.MainWindow;
                InitializeComponent();
                MessageToUser.Text = message;
                if (!IsActive)
                    ShowDialog();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
        #endregion
    }
}
