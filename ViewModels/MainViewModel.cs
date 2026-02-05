using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tasky.Models;
using Tasky.Database;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows;
using Tasky.Services.Socket;
namespace Tasky.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        #region Properties
        private UserOp UserOp { get; } // Maybe you don't need, just to take the username.

        private ObservableCollection<Models.Task> _tasksToShow;

        public ObservableCollection<Models.Task> TasksToShow
        {

            get
            {
                _tasksToShow = DB.GetAllTasks().Result;
                return _tasksToShow;
            }

            set
            {
                if (value != _tasksToShow)
                {
                    _tasksToShow = value;
                    OnPropertyChanged("TasksToShow");
                }
            }
        }

        public Tasky.Database.Database DB { get; }

        public string Presentation
        {
            get => UserOp.Presentation;
        }

        #endregion

        #region CTOR
        public MainViewModel()
        {
            // Read comments in properties to know better.

            UserOp = new UserOp();
            DB = new Tasky.Database.Database();

            SocketClient.RegisterNewEvent("HasChangedEvent");

            SocketClient.OnActionReceived = (action) =>
            {
                if (action.Contains("HasChanged"))                
                    ReloadTasksToShow();                
            };
        }
        #endregion

        #region Functions
        public void ReloadTasksToShow()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                TasksToShow = DB.GetAllTasks().Result;
            });
        }
        #endregion
    }
}
