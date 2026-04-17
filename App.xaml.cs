using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using NLog;
using Tasky.Services.Socket;
using Tasky.ViewModels;
using Tasky.Views;

namespace Tasky
{
    public partial class App : Application
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private const string ApiHealthCheckUrl = "http://localhost:3000/tasks/healthcheck";
        private const int ConnectionRetryDelayMs = 10000;

        private async void StartUpTasky(object sender, StartupEventArgs e)
        {
            using (var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(5) })
            {
                while (!await IsApiAvailable(httpClient))
                {
                    MessageBox.Show(
                        "API não está disponível. Tentando reconectar em 10 segundos...",
                        "Erro de Conexão",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    await Task.Delay(ConnectionRetryDelayMs);
                }
            }

            try
            {
                await SocketClient.InitializeSocket();
                var mainVm = new MainViewModel();
                var window = new TasksHomePage(mainVm);
                window.Show();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Failed to initialize application");
                MessageBox.Show(
                    "Erro ao inicializar aplicação. Verifique os logs.",
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Shutdown();
            }
        }

        private static async Task<bool> IsApiAvailable(HttpClient client)
        {
            try
            {
                var response = await client.GetAsync(ApiHealthCheckUrl);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                logger.Debug(ex, "API health check failed");
                return false;
            }
        }
    }
}
