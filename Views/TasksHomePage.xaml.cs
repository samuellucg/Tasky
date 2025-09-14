using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using Tasky.Models;
using Tasky.ViewModels;
using Tasky.Views.Utils;

namespace Tasky.Views
{
    /// <summary>
    /// Lógica interna para TasksHomePage.xaml
    /// </summary>
    public partial class TasksHomePage : Window
    {
        private MainViewModel _vm;

        private string _selectedTaskName;
        private string _selectedTaskDesc;
        private DateTime _selectedTaskDate;

        public TasksHomePage(MainViewModel vm)
        {
            InitializeComponent();
            this.DataContext = vm;
            _vm = vm;
        }

        public void AddTask(object sender, RoutedEventArgs e)
        {
            var modal = new AddModal(_vm);
            modal.ShowDialog();
        }

        public void EditTask(object sender, RoutedEventArgs e)
        {
            var previewTask = (sender as Button).DataContext as Models.Task;
            
            if (_selectedTaskName == null && _selectedTaskDesc == null)
            {
                _selectedTaskName = previewTask.TaskName;
                _selectedTaskDesc = previewTask.TaskDesc;
                _selectedTaskDate = previewTask.HourTask;
            }

            
            if (!previewTask.IsEditingTask)
            {
                previewTask.IsEditingTask = true;
                previewTask.CanChange = false;
            }

            else if (previewTask.IsEditingTask)
            {
                previewTask.CanChange = true;
                previewTask.IsEditingTask = false;
                _selectedTaskName = null;
                _selectedTaskDesc = null;

                ReloadEditList(sender);

            }

            else
            {
                previewTask.IsEditingTask = false;
                previewTask.TaskName = _selectedTaskName;
                previewTask.TaskDesc = _selectedTaskDesc;
                _selectedTaskName = null;
                _selectedTaskDesc = null;
            }
        }

        private void ReloadEditList(object sender)
        {
            DependencyObject test = (sender as Button).Parent;

            if (test is Canvas canvas)
            {
                foreach (var child in LogicalTreeHelper.GetChildren(canvas))
                {
                    if (child is TextBox tb)
                    {
                        tb.GetBindingExpression(TextBox.TextProperty).UpdateTarget(); // Quando o update source trigger é explicit. Precisamos "puxar" as informações dessa forma para isso o update target. No caso o reload
                    }
                }
            }
        }

        public void SaveChanges(object sender, RoutedEventArgs e)
        {
            var newTask = (sender as Button).DataContext as Models.Task;
            var keepEditing = false;
            newTask.CanChange = true;
            var dateToSaveStr = string.Empty;
            var hourToSaveStr = string.Empty;

            var parentClass = (sender as Button).Parent;

            var toUpdate = new List<TextBox>();

            if (parentClass is Canvas canvas)
            {
                foreach (var child in LogicalTreeHelper.GetChildren(canvas))
                {
                    if (child is TextBox tb)
                    {
                        if (tb.Name == "NameToSave")
                        {
                            toUpdate.Add(tb);
                            newTask.TaskName = tb.Text;
                        }
                        if (tb.Name == "DescToSave")
                        {
                            toUpdate.Add(tb);
                            newTask.TaskDesc = tb.Text;
                        }
                        if (tb.Name == "DateToSave")
                        {
                            toUpdate.Add(tb);
                            dateToSaveStr = tb.Text;
                        }
                        if (tb.Name == "HourToSave")
                        {
                            toUpdate.Add(tb);
                            hourToSaveStr = tb.Text;
                        }


                    }
                }

                var validDate = DateTime.Parse(($"{dateToSaveStr} {hourToSaveStr}:00"));

                if (validDate < DateTime.Now)
                {
                    new MessageWarning(string.Format("A data/hora deve ser posterior a\n              {0}", DateTime.Now.ToString()));
                }

                else
                {
                    foreach (var i in toUpdate)
                    {
                        i.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                    }

                    keepEditing = true;
                    newTask.HourTask = validDate;
                }
            }

            if (keepEditing)
            {
                newTask.CanChange = false;

                if (newTask.TaskName != _selectedTaskName || newTask.TaskDesc != _selectedTaskDesc || newTask.HourTask != _selectedTaskDate)
                {
                    _vm.DB.UpdateTask(_selectedTaskDesc, _selectedTaskName, newTask.TaskDesc, newTask.TaskName, newTask.HourTask, _selectedTaskDate);
                    MessageConfirmation message = new MessageConfirmation("Edições salvas", false);
                    message.ShowDialog();
                    EditTask(sender, e);
                }
                else if (newTask.TaskName == _selectedTaskName && newTask.TaskDesc == _selectedTaskDesc && newTask.HourTask == _selectedTaskDate)
                {
                    MessageConfirmation message = new MessageConfirmation("Elas são as mesmas, não há porque salvar", false);
                    message.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Erro desconhecido");
                }
            }
        }

        public void DeleteTask(object sender, RoutedEventArgs e)
        {
            var taskToDelete = (sender as Button).DataContext as Models.Task;

            MessageConfirmation userInput = new MessageConfirmation("Tem certeza que deseja \n    deletar essa tarefa?");
            bool? choice = userInput.ShowDialog();

            if (choice is true)
            {
                if (userInput.UserResponse)
                {
                    var newTasks = _vm.DB.DeleteTask(taskToDelete);
                    if (newTasks.Result)
                    {
                        _vm.TasksToShow = _vm.DB.GetAllTasks().Result;
                        MessageConfirmation message = new MessageConfirmation("Tarefa deletada", false);
                        message.ShowDialog();
                    }

                }
                else
                {
                    MessageConfirmation message = new MessageConfirmation("Operação cancelada", false);
                    message.ShowDialog();
                }
            }
            else
            {
                MessageBox.Show("Erro desconhecido.");
            }
        }
    }
}
