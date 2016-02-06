using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Web.Script.Serialization;

namespace DashboardApp.Config
{
    static class ConfigStore
    {

        public static MySettingsConfig settingsStore;

        //public static MyTasksConfig tasksStore;
        public static void LoadAll()
        {
            settingsStore = MySettingsConfig.Load();
            //tasksStore = MyTasksConfig.Load("tasks.jsn");
        }

        public static void Save()
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
                settingsStore.Save();
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
                string s = ConfigStore.settingsStore.UserSettings_DefaultTextEditor;
                if (s.Length == 0)
                {
                    //Set the default if blank
                    ConfigStore.settingsStore.UserSettings_DefaultTextEditor = "notepad";
                    return "notepad";
                }
                else {
                    return s;
                }

            }
            set { ConfigStore.settingsStore.UserSettings_DefaultTextEditor = value; }
        }

        #endregion

        // dashboard.jsn
        public class MySettingsConfig : AppSettings<MySettingsConfig>
        {
            public string App_SuppressMinimiseMessage = "";
            // TODO: Change to MB (integer)
            public string Startup_Memory = "1G";
            // TODO: Change to MB (integer)
            public string Startup_MemoryMin = "512M";
            public string Startup_JavaExec = "";
            public string UserSettings_DefaultTextEditor = "";
            public string Jarfile = "";
            public string LaunchArgu_JAVA = "";
            public string JarLaunchArguments = "";
            public string Startup_JavaSpecificArgs = "";
            public List<string> ProfileDir_ExcludedDirectories = new List<string> {
                "world-backups",
                "plugins",
                "crash-reports",
                "logs",
                "config",
                "libraries",
                "mods",
                "System Volume Information"
            };
        }

        // tasks.jsn
        public class MyTasksConfig : AppSettings<MyTasksConfig>
        {


            public int schemaVersion = 1;

            //public List<TaskScheduler.Task> TaskList = new List<TaskScheduler.Task>();
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