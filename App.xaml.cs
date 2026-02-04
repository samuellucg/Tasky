using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Tasky.Services.Socket;
using Tasky.ViewModels;
using Tasky.Views;

namespace Tasky
{
    /// <summary>
    /// Interação lógica para App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Attributes
        private static bool isApiOn = false;
        #endregion

        #region Initialization
        private async void StartUpTasky(object sender, StartupEventArgs e)
        {
            HttpClient isApiOnClient = new HttpClient();
            isApiOnClient.Timeout = TimeSpan.FromSeconds(5);

            while (!isApiOn)
            {
                try
                {
                    var response = await isApiOnClient.GetAsync("http://localhost:3000/tasks/healthcheck");

                    if (response.IsSuccessStatusCode)
                    {
                        isApiOn = true;
                    }
                    else
                    {
                        MessageBox.Show("API ERROR", "Api it's not on, trying to connect in 10 seconds...");
                        await Task.Delay(10000);
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("\nAPI it's offline, trying again in 10 seconds...");
                    MessageBox.Show("Api it's not on, trying to connect in 10 seconds...", "API ERROR");
                    await Task.Delay(10000);
                }
            }

            await SocketClient.InitializeSocket();
            var mainVm = new MainViewModel();
            var window = new TasksHomePage(mainVm);
            window.Show();
        }
        #endregion
    }
}
