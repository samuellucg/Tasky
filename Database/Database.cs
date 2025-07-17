using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using Tasky.Models;

namespace Tasky.Database
{
    public class Database : IDisposable
    {

        public void Dispose()
        {
        }
        
        public static string fileName = "Database.json";

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
                Models.Task toCreate = new Models.Task("Estudar", false, "Estudar programação", TimeSpan.FromDays(5));
                CreateTask(toCreate);
            }
        }
        public void CreateTask(Models.Task task)
        {
            var tasks = GetAllTasks();
            tasks.Add(task);
            SaveAll(tasks);
        }

        public ObservableCollection<Models.Task> GetAllTasks()
        {
            var content = File.ReadAllText(fileName);   
            //return JsonConvert.DeserializeObject<List<Models.Task>>(content) ?? new List<Models.Task>();
            return JsonConvert.DeserializeObject<ObservableCollection<Models.Task>>(content) ?? new ObservableCollection<Models.Task>();
        }

        public void SaveAll(ObservableCollection<Models.Task> tasks)
        {
            var content = JsonConvert.SerializeObject(tasks, Formatting.Indented);
            File.WriteAllText(fileName,content);
            GetAllTasks();
        }

        public void UpdateTask(string oldDesc, string oldName, string newDesc, string newName)
        {
            var tasks = GetAllTasks();

            var xt = tasks.FirstOrDefault(x => x.TaskName == oldName && x.TaskDesc == oldDesc);
            if (xt != null)
            {
                xt.TaskName = newName;
                xt.TaskDesc = newDesc;
                File.WriteAllText(fileName, JsonConvert.SerializeObject(tasks,Formatting.Indented));
            }
            else
            {
                MessageBox.Show("Error");
            }


        }

        public ObservableCollection<Models.Task> DeleteTask(Models.Task taskToDelete)
        {
            var tasks = GetAllTasks().Where(x => x.TaskName != taskToDelete.TaskName && x.TaskDesc != taskToDelete.TaskDesc);           
            SaveAll(new ObservableCollection<Models.Task>(tasks));
            return GetAllTasks();
        }
    }
}
