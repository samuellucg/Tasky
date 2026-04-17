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
        #region Attributes

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public static int size; // verificar necessidade

        private static HttpClient client;
        #endregion

        #region Constructor
        public Database()
        {
            MapHttp();
        }
        #endregion

        #region Functions

        private void MapHttp()
        {
            try
            {

                client = new HttpClient();
                client.BaseAddress = new Uri("http://localhost:3000/");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public async Task<bool> CreateTask(Models.Task task)
        {
            try
            {
                //size += 1;
                //task.TaskId = size;
                var payload = new
                {
                    chatId = 1,
                    body = task
                };
                var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("tasks/", content).ConfigureAwait(false);
                if (response.EnsureSuccessStatusCode().StatusCode == System.Net.HttpStatusCode.Created)
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
                var response = await client.GetAsync("tasks/from-user?userId=1").ConfigureAwait(false);
                if (response.EnsureSuccessStatusCode().StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    size = JsonConvert.DeserializeObject<ObservableCollection<Models.Task>>(content).OrderBy(x => x.TaskId).Select(x => x.TaskId).Last();
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

        public async Task<bool> UpdateTask(int taskId, string newName, string newDesc, DateTime newDate, bool newNotifyTask, bool newTaskDone)
        {
            try
            {
                var payload = new Dictionary<string, dynamic>
                {
                    { "chatId", 1 },
                    { "TaskId", taskId },
                    { "TaskName", newName },
                    { "TaskDesc", newDesc },
                    { "HourTask", newDate },
                    { "TaskDone", newTaskDone },
                    { "NotifyTask", newNotifyTask},
                };
                var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
                var response = await client.PutAsync("tasks/", content).ConfigureAwait(false);
                Console.WriteLine(response.StatusCode);
                return response.IsSuccessStatusCode;
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

        public async Task<bool> DeleteTask(Models.Task taskToDelete)
        {
            try
            {
                var response = await client.DeleteAsync($"tasks/{taskToDelete.TaskId}?userId=1").ConfigureAwait(false);
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
        #endregion

        #region Dispose
        public void Dispose()
        {
        }
        #endregion
    }
}
