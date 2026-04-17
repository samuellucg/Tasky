using System.Windows;
using NLog;

namespace Tasky.Views.Utils
{
    public partial class MessageWarning : Window
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public MessageWarning(string message = "Tarefa criada com sucesso!")
        {
            try
            {
                this.Owner = Application.Current.MainWindow;
                InitializeComponent();
                MessageToUser.Text = message;
            }
            catch (System.Exception ex)
            {
                logger.Error(ex);
            }
        }

        protected override void OnContentRendered(System.EventArgs e)
        {
            base.OnContentRendered(e);
            if (!IsActive)
                ShowDialog();
        }
    }
}
