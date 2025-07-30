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
            //var testTask = (sender as TextBox).Text;

            if (_selectedTaskName == null && _selectedTaskDesc == null)
            {
                _selectedTaskName = previewTask.TaskName;
                _selectedTaskDesc = previewTask.TaskDesc;
            }

            //if (_selectedTaskName != null && _selectedTaskDesc != null)
            //{
            //    previewTask.TaskName = _selectedTaskName;
            //    previewTask.TaskDesc = _selectedTaskDesc;
            //}

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
            newTask.CanChange = true;
            var test = (sender as Button).Parent;
            DependencyObject parent = test;

            if (parent is Canvas canvas)
            {
                foreach (var child in LogicalTreeHelper.GetChildren(canvas))
                {
                    if (child is TextBox tb)
                    {
                        if (tb.Name == "NameToSave")
                        {
                            newTask.TaskName = tb.Text;
                        }
                        if (tb.Name == "DescToSave")
                        {
                            newTask.TaskDesc = tb.Text;
                        }

                        tb.GetBindingExpression(TextBox.TextProperty).UpdateSource(); // Quando o update source trigger é explicit. Precisamos atualizar dessa forma para isso o update source.
                    }
                }
            }

            newTask.CanChange = false;



            if (newTask.TaskName != _selectedTaskName || newTask.TaskDesc != _selectedTaskDesc)
            {
                //newTask.CanChange = true;
                _vm.DB.UpdateTask(_selectedTaskDesc, _selectedTaskName, newTask.TaskDesc, newTask.TaskName);
                //MessageBox.Show("Edições salvas"); // Mudar isso para um modal criado por você.
                MessageConfirmation message = new MessageConfirmation("Edições salvas", false);
                message.ShowDialog();
                EditTask(sender, e);
            }
            else if (newTask.TaskName == _selectedTaskName && newTask.TaskDesc == _selectedTaskDesc)
            {
                MessageConfirmation message = new MessageConfirmation("Elas são as mesmas, não há porque salvar", false);
                message.ShowDialog();
                //MessageBox.Show("Elas são as mesmas, não há porque salvar"); // Mudar isso para um modal criado por você.
            }
            else
            {
                MessageBox.Show("Erro desconhecido"); // Mudar isso para um modal criado por você.
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
                    _vm.TasksToShow = newTasks;
                    //MessageBox.Show("Tarefa deletada"); // Mudar isso para um modal criado por você.
                    MessageConfirmation message = new MessageConfirmation("Tarefa deletada", false);
                    message.ShowDialog();

                }
                else
                {
                    //MessageBox.Show("Operação cancelada"); // Mudar isso para um modal criado por você.
                    MessageConfirmation message = new MessageConfirmation("Operação cancelada", false);
                    message.ShowDialog();
                }
            }
            else
            {
                MessageBox.Show("Erro desconhecido."); // Mudar isso para um modal criado por você.
            }
        }
    }
}
