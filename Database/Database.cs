using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using NLog;
using Tasky.Models;
using Tasky.ViewModels;

namespace Tasky.Database
{
    public class Database : IDisposable
    {
        public void Dispose()
        {
        }


        //public static string fileName = "Database.json";
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public static int size;

        private static HttpClient client;


        public Database()
        {
            MapHttp();
            //WatchDatabase();
        }

        private void MapHttp()
        {
            try
            {

                client = new HttpClient();
                client.BaseAddress = new Uri("http://localhost:3000/tasks");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        //private void WatchDatabase()
        //{
        //    if (!File.Exists(fileName))
        //    {
        //        File.WriteAllText(fileName, "[]");
        //    }

        //    ReadJson();
        //}

        //private void ReadJson()
        //{
        //    if (File.Exists(fileName) && File.ReadAllLines(fileName).Length <= 2)
        //    {
        //        //DateTime data = string.Format("{0}/{1}/{2}", DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);
        //        Models.Task toCreate = new Models.Task("Estudar", false, "Estudar programação", DateTime.Now.Date);
        //        CreateTask(toCreate);
        //    }
        //}

        public async Task<bool> CreateTask(Models.Task task)
        {
            try
            {
                size += 1;
                task.TaskId = size;
                var content = new StringContent(JsonConvert.SerializeObject(task), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(client.BaseAddress, content).ConfigureAwait(false);
                if (response.EnsureSuccessStatusCode().StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex) when (ex is HttpRequestException || ex is SocketException || ex is WebException)
            {
                MessageBox.Show("Application gonna restart");
                Process.Start(Assembly.GetExecutingAssembly().Location);
                Environment.Exit(0);
                throw;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return false;
            }
        }

        public async Task<ObservableCollection<Models.Task>> GetAllTasks()
        {
            try
            {
                var response = await client.GetAsync(client.BaseAddress).ConfigureAwait(false);
                if (response.EnsureSuccessStatusCode().StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    size = JsonConvert.DeserializeObject<ObservableCollection<Models.Task>>(content).OrderBy(x => x.TaskId).Select(x => x.TaskId).Last();
                    //return JsonConvert.DeserializeObject<ObservableCollection<Models.Task>>(content) ?? new ObservableCollection<Models.Task>();                  
                    return JsonConvert.DeserializeObject<ObservableCollection<Models.Task>>(content) ?? new ObservableCollection<Models.Task>();
                }
                throw new Exception();
            }
            catch (Exception ex) when (ex is HttpRequestException || ex is SocketException || ex is WebException)
            {
                MessageBox.Show("Application gonna restart");
                Process.Start(Assembly.GetExecutingAssembly().Location);
                Environment.Exit(0);
                throw;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
        }

        //public void SaveAll(ObservableCollection<Models.Task> tasks)
        //{
        //    var content = JsonConvert.SerializeObject(tasks, Formatting.Indented);
        //    File.WriteAllText(fileName, content);
        //    GetAllTasks();
        //}

        public async void UpdateTask(string oldDesc, string oldName, string newDesc, string newName, DateTime newDate, DateTime oldDate, bool? oldNotifyTask, bool newNotifyTask)
        {
            try
            {
                var tasks = await GetAllTasks();
                var xt = tasks.FirstOrDefault(x => x.TaskName == oldName && x.TaskDesc == oldDesc && x.HourTask == oldDate && x.NotifyTask == oldNotifyTask);

                if (xt != null)
                {
                    var payload = new Dictionary<string, dynamic>
                    {
                        { "TaskName", newName },
                        { "TaskDesc", newDesc },
                        { "HourTask", newDate },
                        { "NotifyTask", newNotifyTask},
                    };
                    var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
                    var response = await client.PutAsync($"{client.BaseAddress}?taskId={xt.TaskId}", content).ConfigureAwait(false);
                    response.EnsureSuccessStatusCode();
                }
                else
                {
                    MessageBox.Show("Error");
                }

            }
            catch (Exception ex) when (ex is HttpRequestException || ex is SocketException || ex is WebException)
            {
                MessageBox.Show("Application gonna restart");
                Process.Start(Assembly.GetExecutingAssembly().Location);
                Environment.Exit(0);
                throw;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public async Task<bool> DeleteTask(Models.Task taskToDelete)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(taskToDelete), Encoding.UTF8, "application/json");
                var response = await client.DeleteAsync($"{client.BaseAddress}?taskId={taskToDelete.TaskId}").ConfigureAwait(false);
                if (response.EnsureSuccessStatusCode().StatusCode == System.Net.HttpStatusCode.OK)
                {
                    //client.Dispose();
                    return true;
                }
                return false;
            }
            catch (Exception ex) when (ex is HttpRequestException || ex is SocketException || ex is WebException)
            {
                MessageBox.Show("Application gonna restart");
                Process.Start(Assembly.GetExecutingAssembly().Location);
                Environment.Exit(0);
                throw;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return false;
            }
        }
    }
}
