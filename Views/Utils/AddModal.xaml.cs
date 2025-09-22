using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
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
using NLog;
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
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public AddModal(MainViewModel actualTasks)
        {
            this.Owner = Application.Current.MainWindow;
            InitializeComponent();
            _viewModel = actualTasks;
        }

        private async void SubmitInfo(object sender, RoutedEventArgs e)
        {
            try
            {
                var validDate = taskHour.Text == "__:__" ? DateTime.Parse(taskDate.Text) : DateTime.Parse(string.Format("{0} {1}", taskDate.Text, taskHour.Text));
                var actualDate = DateTime.Now;
                if (taskName.Text.Length > 1 && taskDesc.Text.Length > 1 && validDate > actualDate) // validDate.Year >= actualDate.Year && validDate.Month >= actualDate.Month && validDate.Day >= actualDate.Day && 
                {
                    var taskCreated = new Models.Task(taskName.Text, taskNot.IsChecked.Value, taskDesc.Text, validDate); // fazer campo pra mandar data.
                    if (taskCreated != null)
                    {
                        MessageConfirmation userInput = new MessageConfirmation("Tem certeza que deseja prosseguir?");
                        if (!userInput.IsActive)
                        {
                            bool? hasShowed = userInput.ShowDialog();
                            if (hasShowed is true)
                            {
                                bool userChoice = userInput.UserResponse;

                                if (userChoice)
                                {
                                    using (Tasky.Database.Database DB = new Tasky.Database.Database())
                                    {
                                        if (await DB.CreateTask(taskCreated))
                                            _viewModel.TasksToShow = await DB.GetAllTasks();
                                        else
                                            new MessageWarning("Erro ao criar tarefa");
                                    }
                                    taskName.Text = taskDesc.Text = null;
                                    taskNot.IsChecked = false;

                                    new MessageWarning("Operação realizada");
                                    Close();
                                }
                                else
                                {
                                    new MessageWarning("Operação abortada");
                                    //Close();
                                }
                            }
                        }
                        else
                            MessageBox.Show("THREAD ERROR");
                    }
                }

                else if (validDate < actualDate) // validDate.Year < actualDate.Year || validDate.Month < actualDate.Month || validDate.Day < actualDate.Day
                {
                    //new MessageWarning(string.Format("Deve ser salvo para anos\nentre {0} ou mais.", actualDate.Year.ToString()));
                    new MessageWarning(string.Format("A data/hora deve ser posterior a\n              {0}", actualDate.ToString()));
                }

                else
                {
                    new MessageWarning("Erro ao salvar");
                    //Close();
                }
            }
            catch (System.FormatException)
            {
                new MessageWarning("Data inválida, insira novamente.");
                //Close();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

        }

        private void CloseModal(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
