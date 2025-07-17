using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tasky.Database;
using Tasky.Models;
using Tasky.ViewModels;

namespace Tasky.Views.Utils
{
    /// <summary>
    /// Interação lógica para AddModal.xam
    /// </summary>
    public partial class AddModal : Window
    {
        private MainViewModel _viewModel;

        public AddModal(MainViewModel actualTasks)
        {
            this.Owner = Application.Current.MainWindow;
            InitializeComponent();
            _viewModel = actualTasks;
        }

        private void SubmitInfo(object sender, RoutedEventArgs e)
        {
            if (taskName.Text.Length > 1 && taskDesc.Text.Length > 1)
            {
                var taskCreated = new Models.Task(taskName.Text, taskNot.IsChecked.Value, taskDesc.Text, TimeSpan.Zero);
                if (taskCreated != null)
                {
                    MessageConfirmation userInput = new MessageConfirmation("Tem certeza que deseja prosseguir?");
                    bool? hasShowed = userInput.ShowDialog();
                    if (hasShowed is true)
                    {
                        bool userChoice = userInput.UserResponse;

                        if (userChoice)
                        {
                            using (Tasky.Database.Database DB = new Tasky.Database.Database())
                            {
                                DB.CreateTask(taskCreated);
                                _viewModel.TasksToShow = DB.GetAllTasks();
                            }
                            taskName.Text = taskDesc.Text = null;
                            taskNot.IsChecked = false;

                            new MessageWarning("Operação realizada");
                            Close();
                        }
                        else
                        {
                            new MessageWarning("Operação abortada");
                            Close();
                        }
                    }
                }
            }
            else
            {
                new MessageWarning("Erro ao salvar");
            }

        }

        private void CloseModal(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
