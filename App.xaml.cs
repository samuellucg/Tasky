using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Tasky.ViewModels;
using Tasky.Views;

namespace Tasky
{
    /// <summary>
    /// Interação lógica para App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void StartUpTasky(object sender, StartupEventArgs e)
        {
            var mainVm = new MainViewModel();

            var window = new TasksHomePage(mainVm);
            window.Show();
        }
    }
}
