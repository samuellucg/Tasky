using System;
using System.Windows;
using System.Windows.Controls;
using NLog;
using Tasky.Database;
using Tasky.Models;
using Tasky.ViewModels;

namespace Tasky.Views.Utils
{
    public partial class AddModal : Window
    {
        private MainViewModel _viewModel;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public AddModal(MainViewModel actualTasks)
        {
            Owner = Application.Current.MainWindow;
            InitializeComponent();
            _viewModel = actualTasks;
        }

        private async void SubmitInfo(object sender, RoutedEventArgs e)
        {
            try
            {
                var validDate = ParseDateTime(taskDate.Text, taskHour.Text);
                var actualDate = DateTime.Now;

                if (string.IsNullOrWhiteSpace(taskName.Text) ||
                    string.IsNullOrWhiteSpace(taskDesc.Text) ||
                    taskName.Text.Length <= 1 ||
                    taskDesc.Text.Length <= 1)
                {
                    ShowWarning("Por favor, preencha todos os campos corretamente.");
                    return;
                }

                if (validDate <= actualDate)
                {
                    ShowWarning($"A data/hora deve ser posterior a {actualDate:dd/MM/yyyy HH:mm}");
                    return;
                }

                var taskCreated = new Models.Task(taskName.Text, taskNot.IsChecked ?? false, taskDesc.Text, validDate);

                var userInput = new MessageConfirmation("Tem certeza que deseja prosseguir?");
                if (userInput.ShowDialog() == true && userInput.UserResponse)
                {
                    using (var DB = new Database.Database())
                    {
                        if (await DB.CreateTask(taskCreated))
                        {
                            _viewModel.TasksToShow = await DB.GetAllTasks();
                            ShowWarning("Tarefa criada com sucesso!");
                            ClearFields();
                            Close();
                        }
                        else
                        {
                            ShowWarning("Erro ao criar tarefa. Verifique a conexão com a API.");
                        }
                    }
                }
            }
            catch (FormatException)
            {
                ShowWarning("Data inválida. Use o formato DD/MM/AAAA HH:mm");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                ShowWarning("Erro inesperado. Consulte os logs.");
            }
        }

        private DateTime ParseDateTime(string dateText, string hourText)
        {
            if (string.IsNullOrWhiteSpace(dateText) || dateText.Contains("_"))
                throw new FormatException("Data incompleta");

            if (hourText == "__:__" || string.IsNullOrWhiteSpace(hourText) || hourText.Contains("_"))
                return DateTime.Parse(dateText);

            return DateTime.Parse($"{dateText} {hourText}");
        }

        private void ClearFields()
        {
            taskName.Clear();
            taskDesc.Clear();
            taskDate.Clear();
            taskHour.Clear();
            taskNot.IsChecked = false;
        }

        private static void ShowWarning(string message)
        {
            var warning = new MessageWarning(message);
            warning.ShowDialog();
        }

        private void CloseModal(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
