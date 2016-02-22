using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Web.Script.Serialization;

namespace DashboardApp.Config
{
    public class ConfigStore : INotifyPropertyChanged
    {
        public ConfigStore()
        {
            SettingsJson = MySettingsConfig.Load();
            //tasksStore = MyTasksConfig.Load("tasks.jsn");
        }

        // TODO
        public string WorkingDirectory { get { return System.Environment.CurrentDirectory; } }

        public event PropertyChangedEventHandler PropertyChanged;

        //public static MyTasksConfig tasksStore;

        public void Save()
        {
            // Save the current list of scheduled tasks
            //tasksStore.TaskList.Clear();
            //foreach (task_loopVariable in navpageScheduler.ListOfSchedulerTaskItem)
            //{
            //    task = task_loopVariable;
            //    tasksStore.TaskList.Add(task.Task);
            //}
            try
            {
                SettingsJson.Save();
                //tasksStore.Save("tasks.jsn");
            }
            catch (Exception ex)
            {
                Debug.Print("Could not write the Dashboard configuration to disk. Make sure you extracted Dashboard!");
            }

        }

        #region "Dashboard customisation"

        /// <summary>
        /// Default text editor
        /// </summary>
        public static string UserSettings_DefaultTextEditor
        {
            get
            {
                string s = ConfigStore.UserSettings_DefaultTextEditor;
                if (s.Length == 0)
                {
                    //Set the default if blank
                    ConfigStore.UserSettings_DefaultTextEditor = "notepad";
                    return "notepad";
                }
                else {
                    return s;
                }

            }
            set { ConfigStore.UserSettings_DefaultTextEditor = value; }
        }

        #endregion

        // dashboard.jsn instance object
        public MySettingsConfig SettingsJson;
        
        // dashboard.jsn
        public class MySettingsConfig : AppSettings<MySettingsConfig>, INotifyPropertyChanged
        {
            
            // TODO
            public string App_SuppressMinimiseMessage = "";

            // Xmx parameter, in MB
            public string Startup_Memory { get; set; } = "1G";

            // Xms parameter, in MB
            public string Startup_MemoryMin { get; set; } = "512M";

            // Custom path to Java.exe
            public string JavaJRE = "";

            // Custom path to Notepad.exe
            public string DefaultTextEditor = "";

            // Filename of minecraft_server.xx.jar
            public string Jarfile = "";

            // Optional Java arguments, e.g. GC parameters
            //public string LaunchArgu_JAVA = "";
            //public string Startup_JavaSpecificArgs = "";
            public string JavaArguments { get; set; } = "";

            // Optional Minecraft server arguments (e.g. CraftBukkit specific parameters)
            public string JarfileArguments { get; set; } = "";

            // Folders to ignore when scanning for "worlds" to backup
            public List<string> ExcludedDirectories = new List<string> {
                "world-backups",
                "plugins",
                "crash-reports",
                "logs",
                "config",
                "libraries",
                "mods",
                "System Volume Information"
            };

            public event PropertyChangedEventHandler PropertyChanged;
        }

        // tasks.jsn
        // TODO
        public class MyTasksConfig : AppSettings<MyTasksConfig>
        {
            public int SchemaVersion = 1;

            //public List<TaskScheduler.Task> TaskList = new List<TaskScheduler.Task>();
        }

        public bool JarFileExists()
        {
            System.Diagnostics.Debug.Print("JarFile: " + SettingsJson.Jarfile);
            return System.IO.File.Exists(System.Environment.CurrentDirectory + @"\" + SettingsJson.Jarfile);
        }
    }

}

// Store the app's settings in a local file, instead of the previous My.Settings implementation
// To allow saving the scheduled tasks, and simplifies the storage for multiple instances of settings
// http://stackoverflow.com/questions/8688724/how-to-store-a-list-of-objects-in-application-settings
public class AppSettings<T> where T : new()
{
    private const string DEFAULT_CONFIG_FILENAME = "dashboard.jsn";

    //Serialize app settings to JSON format
    public void Save(string fileName = DEFAULT_CONFIG_FILENAME)
    {
        File.WriteAllText(fileName, (new JavaScriptSerializer()).Serialize(this));
    }

    public static void Save(T pSettings, string fileName = DEFAULT_CONFIG_FILENAME)
    {
        File.WriteAllText(fileName, (new JavaScriptSerializer()).Serialize(pSettings));
    }

    public static T Load(string fileName = DEFAULT_CONFIG_FILENAME)
    {
        T t = new T();
        if (File.Exists(fileName))
        {
            t = (new JavaScriptSerializer()).Deserialize<T>(File.ReadAllText(fileName));
        }
        return t;
    }
}