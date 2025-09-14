using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using Tasky.Models;
using Tasky.ViewModels;

namespace Tasky.Database
{
    public class Database : IDisposable
    {
        private ObservableCollection<Models.Task> _viewModel;

        public void Dispose()
        {
        }

        public static string fileName = "Database.json";

        public static int size;

        public Database()
        {
            WatchDatabase();
        }

        private void WatchDatabase()
        {
            if (!File.Exists(fileName))
            {
                File.WriteAllText(fileName, "[]");
                //readJson();
            }

            ReadJson();
        }

        private void ReadJson()
        {
            if (File.Exists(fileName) && File.ReadAllLines(fileName).Length <= 2)
            {
                //DateTime data = string.Format("{0}/{1}/{2}", DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);
                Models.Task toCreate = new Models.Task("Estudar", false, "Estudar programação", DateTime.Now.Date);
                CreateTask(toCreate);
            }
        }
        public async void CreateTask(Models.Task task)
        {
            //var tasks = await GetAllTasks();
            //tasks.Add(task);
            //SaveAll(tasks);



            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string url = "http://localhost:3000/tasks";
                    //client.Timeout = new TimeSpan(5);
                    size += 1;
                    task.TaskId = size;
                    var content = new StringContent(JsonConvert.SerializeObject(task), Encoding.UTF8, "application/json");
                    //var x = JsonConvert.SerializeObject(task);
                    var response = await client.PostAsync(url, content).ConfigureAwait(false);
                    if (response.EnsureSuccessStatusCode().StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        client.Dispose();

                    }

                }
                catch (Exception)
                {
                    Console.WriteLine("Error");
                }
            }





        }

        public async Task<ObservableCollection<Models.Task>> GetAllTasks()
        {
            //var content = File.ReadAllText(fileName);   
            ////return JsonConvert.DeserializeObject<List<Models.Task>>(content) ?? new List<Models.Task>();
            //return JsonConvert.DeserializeObject<ObservableCollection<Models.Task>>(content) ?? new ObservableCollection<Models.Task>();

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string url = "http://localhost:3000/tasks";
                    //client.Timeout = new TimeSpan(5);
                    var response = await client.GetAsync(url).ConfigureAwait(false);
                    response.EnsureSuccessStatusCode();

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        size = JsonConvert.DeserializeObject<ObservableCollection<Models.Task>>(content).OrderBy(x => x.TaskId).Select(x => x.TaskId).Last();
                        client.Dispose();
                        return JsonConvert.DeserializeObject<ObservableCollection<Models.Task>>(content) ?? new ObservableCollection<Models.Task>();
                    }

                    throw new Exception();
                }
                catch (Exception)
                {
                    throw;
                }
            }



        }

        public void SaveAll(ObservableCollection<Models.Task> tasks)
        {
            var content = JsonConvert.SerializeObject(tasks, Formatting.Indented);
            File.WriteAllText(fileName, content);
            GetAllTasks();
        }

        public async void UpdateTask(string oldDesc, string oldName, string newDesc, string newName, DateTime newDate, DateTime oldDate)
        {
            //var tasks = await GetAllTasks();

            //var xt = tasks.FirstOrDefault(x => x.TaskName == oldName && x.TaskDesc == oldDesc && x.HourTask == oldDate);
            //if (xt != null)
            //{
            //    xt.TaskName = newName;
            //    xt.TaskDesc = newDesc;
            //    xt.HourTask = newDate;
            //    File.WriteAllText(fileName, JsonConvert.SerializeObject(tasks, Formatting.Indented));
            //}
            //else
            //{
            //    MessageBox.Show("Error");
            //}


            var tasks = await GetAllTasks();
            var xt = tasks.FirstOrDefault(x => x.TaskName == oldName && x.TaskDesc == oldDesc && x.HourTask == oldDate);

            if (xt != null)
            {
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        string url = $"http://localhost:3000/tasks?taskId={xt.TaskId}";
                        var payload = new Dictionary<string, dynamic>
                        {
                            { "TaskName", newName },
                            { "TaskDesc", newDesc },
                            { "HourTask", newDate },

                        };
                        var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
                        var response = await client.PutAsync(url, content).ConfigureAwait(false);
                        response.EnsureSuccessStatusCode();
                        if (response.IsSuccessStatusCode)
                        {
                            client.Dispose();
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else
            {
                MessageBox.Show("Error");
            }
        }

        public async Task<bool> DeleteTask(Models.Task taskToDelete)
        {
            //var t = GetAllTasks();
            //var tasks = await GetAllTasks().Where(x => x.TaskName != taskToDelete.TaskName && x.TaskDesc != taskToDelete.TaskDesc && x.HourTask != taskToDelete.HourTask);           
            //SaveAll(new ObservableCollection<Models.Task>(tasks));
            //return GetAllTasks();


            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string url = $"http://localhost:3000/tasks?taskId={taskToDelete.TaskId}";
                    var content = new StringContent(JsonConvert.SerializeObject(taskToDelete), Encoding.UTF8, "application/json");
                    var response = await client.DeleteAsync(url).ConfigureAwait(false);
                    response.EnsureSuccessStatusCode();
                    if (response.IsSuccessStatusCode)
                    {
                        client.Dispose();
                        return true;
                    }

                    return false;

                }
                catch (Exception)
                {

                    throw;
                }
            }
            throw new Exception();
        }
    }
}
