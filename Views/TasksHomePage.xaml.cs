using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using NLog;
using Tasky.Models;
using Tasky.ViewModels;
using Tasky.Views.Utils;

namespace Tasky.Views
{
    public partial class TasksHomePage : Window
    {
        private MainViewModel _vm;
        private string _originalTaskName;
        private string _originalTaskDesc;
        private DateTime _originalTaskDate;
        private bool _originalTaskNotify;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public TasksHomePage(MainViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
            _vm = vm;
        }

        public void AddTask(object sender, RoutedEventArgs e)
        {
            try
            {
                var modal = new AddModal(_vm);
                modal.ShowDialog();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public void EditTask(object sender, RoutedEventArgs e)
        {
            try
            {
                var task = (sender as Button)?.DataContext as Models.Task;
                if (task == null) return;

                if (!task.IsEditingTask)
                {
                    StoreOriginalValues(task);
                    task.IsEditingTask = true;
                }
                else
                {
                    task.IsEditingTask = false;
                    ClearOriginalValues();
                    ReloadEditList(sender);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private void StoreOriginalValues(Models.Task task)
        {
            _originalTaskName = task.TaskName;
            _originalTaskDesc = task.TaskDesc;
            _originalTaskDate = task.HourTask;
            _originalTaskNotify = task.NotifyTask;
        }

        private void ClearOriginalValues()
        {
            _originalTaskName = null;
            _originalTaskDesc = null;
            _originalTaskNotify = false;
        }

        private void ReloadEditList(object sender)
        {
            try
            {
                var button = sender as Button;
                if (!(button?.Parent is Canvas canvas)) return;

                foreach (var child in LogicalTreeHelper.GetChildren(canvas))
                {
                    if (child is TextBox tb)
                    {
                        tb.GetBindingExpression(TextBox.TextProperty)?.UpdateTarget();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public async void SaveChanges(object sender, RoutedEventArgs e)
        {
            try
            {
                var task = (sender as Button)?.DataContext as Models.Task;
                if (task == null) return;

                if (!TryGetUpdatedValues(sender, task, out DateTime validDate))
                    return;

                if (validDate < DateTime.Now)
                {
                    ShowWarning($"A data/hora deve ser posterior a\n{DateTime.Now:dd/MM/yyyy HH:mm}");
                    return;
                }

                task.HourTask = validDate;

                if (HasChanges(task))
                {
                    await _vm.DB.UpdateTask(task.TaskId, task.TaskName, task.TaskDesc, task.HourTask, task.NotifyTask, task.TaskDone);
                    ShowNotification("Edições salvas");
                    EditTask(sender, e);
                }
                else
                {
                    ShowNotification("Nenhuma alteração detectada");
                }
            }
            catch (FormatException)
            {
                ShowWarning("Data/hora inválida. Use o formato correto.");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private bool TryGetUpdatedValues(object sender, Models.Task task, out DateTime validDate)
        {
            validDate = default;
            var dateStr = string.Empty;
            var hourStr = string.Empty;

            var button = sender as Button;
            if (!(button?.Parent is Canvas canvas)) return false;

            var textBoxes = new List<TextBox>();

            foreach (var child in LogicalTreeHelper.GetChildren(canvas))
            {
                if (child is TextBox tb)
                {
                    textBoxes.Add(tb);
                    switch (tb.Name)
                    {
                        case "NameToSave": task.TaskName = tb.Text; break;
                        case "DescToSave": task.TaskDesc = tb.Text; break;
                        case "DateToSave": dateStr = tb.Text; break;
                        case "HourToSave": hourStr = tb.Text; break;
                    }
                }
                else if (child is ComboBox cb)
                {
                    task.NotifyTask = cb.Text == "True";
                }
            }

            if (string.IsNullOrWhiteSpace(dateStr) || dateStr.Contains("_"))
                throw new FormatException("Data incompleta");

            var dateTimeStr = hourStr == "__:__" || hourStr.Contains("_")
                ? dateStr
                : $"{dateStr} {hourStr}:00";

            validDate = DateTime.Parse(dateTimeStr);

            foreach (var tb in textBoxes)
            {
                tb.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
            }

            return true;
        }

        private bool HasChanges(Models.Task task)
        {
            return task.TaskName != _originalTaskName ||
                   task.TaskDesc != _originalTaskDesc ||
                   task.HourTask != _originalTaskDate ||
                   task.NotifyTask != _originalTaskNotify;
        }

        public async void DeleteTask(object sender, RoutedEventArgs e)
        {
            try
            {
                var taskToDelete = (sender as Button)?.DataContext as Models.Task;
                if (taskToDelete == null) return;

                var userInput = new MessageConfirmation("Tem certeza que deseja deletar essa tarefa?");
                if (userInput.ShowDialog() != true || !userInput.UserResponse)
                {
                    ShowNotification("Operação cancelada");
                    return;
                }

                if (await _vm.DB.DeleteTask(taskToDelete))
                {
                    _vm.TasksToShow = await _vm.DB.GetAllTasks();
                    ShowNotification("Tarefa deletada");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private static void ShowNotification(string message)
        {
            var confirmation = new MessageConfirmation(message, false);
            confirmation.ShowDialog();
        }

        private static void ShowWarning(string message)
        {
            var warning = new MessageWarning(message);
            warning.ShowDialog();
        }
    }
}
