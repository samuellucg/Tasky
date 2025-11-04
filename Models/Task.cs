using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tasky.ViewModels;

namespace Tasky.Models
{
    public class Task : BaseViewModel
    {

        private MainViewModel _viewModel;


        #region Properties

        private string _taskName;

        public string TaskName
        {
            get
            {
                return _taskName;
            }
            set
            {
                if (_taskName != value && _canChange)
                {
                    _taskName = value;
                    OnPropertyChanged("TaskName");
                }
            }
        }

        private bool _notifyTask;

        public bool NotifyTask
        {
            get
            {
                return _notifyTask;
            }
            set
            {
                if (_notifyTask != value)
                {
                    _notifyTask = value;
                    OnPropertyChanged("NotifyTask");
                }
            }
        }
        
        private string _taskDesc;

        public string TaskDesc
        {
            get
            {
                return _taskDesc;
            }
            set
            {
                if (_taskDesc != value && _canChange)
                {
                    _taskDesc = value;
                    OnPropertyChanged("TaskDesc");
                }
            }
        }

        //private TimeSpan _hourTask;

        //public TimeSpan HourTask
        //{
        //    get
        //    {
        //        return _hourTask;
        //    }

        //    set 
        //    {
        //       if (_hourTask != value)
        //       {
        //           _hourTask = value;
        //           OnPropertyChanged("HourTask");
        //       }
        //    }
        //}

        private DateTime _hourTask;

        public DateTime HourTask
        {
            get
            {
                return _hourTask;
            }

            set
            {
                if (_hourTask != value)
                {
                    _hourTask = value;
                    OnPropertyChanged("HourTask");
                }
            }
        }

        //private string _hourTask;

        //public string HourTask
        //{
        //    get
        //    {
        //        return _hourTask;
        //    }

        //    set
        //    {
        //        if (_hourTask != value)
        //        {
        //            _hourTask = value;
        //            OnPropertyChanged("HourTask");
        //        }
        //    }
        //}

        private bool _isEditingTask;

        public bool IsEditingTask
        {
            get => _isEditingTask;

            set
            {
                if (_isEditingTask != value)
                {
                    _isEditingTask = value;
                    OnPropertyChanged("IsEditingTask");
                }
            }
        }

        private bool _canChange = true;

        public bool CanChange
        {
            get => _canChange;
            set
            {
                if (_canChange != value)
                {
                    _canChange = value;
                }
            }
        }

        private int _taskId;

        public int TaskId
        {
            get => _taskId;
            set
            {
                if(_taskId != value)
                {
                    _taskId = value;
                }
            }
        }

        private bool _done = false; // fazer isso ter funcionalidade na tela (tarefa concluida?/não?).

        public bool TaskDone
        {
            get => _done;

            set
            {
                if(_done != value)
                {
                    _done = value;
                }
            }
        }



        #endregion

        public Task(string taskName, bool notifyTask, string taskDesc, DateTime hourTask)
        {
            TaskName = taskName;
            TaskDesc = taskDesc;
            NotifyTask = notifyTask;
            HourTask = hourTask;
        }
    }

}
