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
namespace Tasky.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        #region Properties
        public TaskViewModel TaskVM { get; } // Not used

        private UserOp UserOp { get; } // Maybe you don't need, just to take the username.

        private ObservableCollection<Models.Task> _tasksToShow;

        public ObservableCollection<Models.Task> TasksToShow
        {

            get
            {
                _tasksToShow = DB.GetAllTasks();
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
        } // Tasks in main page

        public Tasky.Database.Database DB { get; } // Database in json (change to sql)        

        public string Presentation
        {
            get => UserOp.Presentation;
        }

        #endregion

        #region CTOR
        public MainViewModel()
        {
            // Read comments in properties to know better.

            TaskVM = new TaskViewModel(); 
            UserOp = new UserOp();
            DB = new Tasky.Database.Database();
        }
        #endregion

    }
}
