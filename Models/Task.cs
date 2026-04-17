using System;
using Tasky.ViewModels;

namespace Tasky.Models
{
    public class Task : BaseViewModel
    {
        private string _taskName;
        private string _taskDesc;
        private DateTime _hourTask;
        private bool _notifyTask;
        private bool _isEditingTask;
        private bool _canChange = true;
        private int _taskId;
        private bool _taskDone;
        private Notify _notifications;

        public string TaskName
        {
            get => _taskName;
            set => SetProperty(ref _taskName, value);
        }

        public string TaskDesc
        {
            get => _taskDesc;
            set => SetProperty(ref _taskDesc, value);
        }

        public DateTime HourTask
        {
            get => _hourTask;
            set => SetProperty(ref _hourTask, value);
        }

        public bool NotifyTask
        {
            get => _notifyTask;
            set => SetProperty(ref _notifyTask, value);
        }

        public bool IsEditingTask
        {
            get => _isEditingTask;
            set => SetProperty(ref _isEditingTask, value);
        }

        public bool CanChange
        {
            get => _canChange;
            set => SetProperty(ref _canChange, value);
        }

        public int TaskId
        {
            get => _taskId;
            set => SetProperty(ref _taskId, value);
        }

        public bool TaskDone
        {
            get => _taskDone;
            set => SetProperty(ref _taskDone, value);
        }

        public Notify Notifications
        {
            get => _notifications;
            set => SetProperty(ref _notifications, value);
        }

        public Task(string taskName, bool notifyTask, string taskDesc, DateTime hourTask)
        {
            TaskName = taskName;
            TaskDesc = taskDesc;
            NotifyTask = notifyTask;
            HourTask = hourTask;
            Notifications = new Notify();
        }

        public class Notify
        {
            public bool Sent15min { get; set; }
            public bool Sent5min { get; set; }

            public Notify()
            {
                Sent15min = false;
                Sent5min = false;
            }
        }
    }
}
