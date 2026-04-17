using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using Tasky.Database;
using Tasky.Models;
using Tasky.Services.Socket;

namespace Tasky.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly UserOp _userOp;
        private ObservableCollection<Models.Task> _tasksToShow;

        public ObservableCollection<Models.Task> TasksToShow
        {
            get => _tasksToShow;
            set => SetProperty(ref _tasksToShow, value);
        }

        public Database.Database DB { get; }

        public string Presentation => _userOp.Presentation;

        public MainViewModel()
        {
            _userOp = new UserOp();
            DB = new Database.Database();

            SocketClient.RegisterNewEvent("HasChangedEvent");
            SocketClient.OnActionReceived += OnSocketActionReceived;

            _ = LoadTasksAsync();
        }

        private void OnSocketActionReceived(string action)
        {
            if (action?.Contains("HasChanged") == true)
                ReloadTasksToShow();
        }

        public async System.Threading.Tasks.Task LoadTasksAsync()
        {
            var tasks = await DB.GetAllTasks();
            Application.Current.Dispatcher.Invoke(() =>
            {
                TasksToShow = tasks ?? new ObservableCollection<Models.Task>();
            });
        }

        public void ReloadTasksToShow()
        {
            Application.Current.Dispatcher.Invoke(async () =>
            {
                TasksToShow = await DB.GetAllTasks() ?? new ObservableCollection<Models.Task>();
            });
        }
    }
}
