using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DashboardApp.Models
{

    public class Config
    {

        /// <summary>
        /// Dashboard App config file format
        /// Compatible with versions 0.x, 1.x
        /// </summary>
        [Serializable]
        public class DashboardAppConfig
        {
            public string DashboardConfigFileVersion = "1";
            public string App_SuppressMinimiseMessage = "";
            public string Startup_Memory = "1G";
            public string Startup_MemoryMin = "512M";
            public string Startup_JavaExec = "";
            public string UserSettings_DefaultTextEditor  = "";
            public string Jarfile = "";
            public string LaunchArgu_JAVA = "";
            public string JarLaunchArguments = "";
            public string Startup_JavaSpecificArgs = "";
            public List<string> ProfileDir_ExcludedDirectories = new List<string> {"world-backups",
                                                                                 "plugins",
                                                                                 "crash-reports",
                                                                                 "logs",
                                                                                 "config",
                                                                                 "libraries",
                                                                                 "mods",
                                                                                 "System Volume Information"};
        }

        public DashboardAppConfig dashboardappconf;

        public bool RestartRequired = false;

        public Config()
        {
            Messenger.Default.Register<Models.Server.Message.ServerStatusChanged>(this, ServerStatusChanged);
            try
            {
                Load();
            }
            catch (Exception)
            {
                dashboardappconf = new DashboardAppConfig();
            }
        }

        private void ServerStatusChanged(Server.Message.ServerStatusChanged obj)
        {
            if (obj.NewState == ServerState.WarmUp) // XXX
                RestartRequired = false;
        }

        public void Load()
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.NullValueHandling = NullValueHandling.Ignore;

            using (StreamReader sr = new StreamReader(@"dashboard.jsn"))
            using (JsonTextReader reader = new JsonTextReader(sr))
            {
                dashboardappconf = (DashboardAppConfig)serializer.Deserialize(reader, typeof(DashboardAppConfig));
            }
        }

        public void Save()
        {
            // force writing version to file
            dashboardappconf.DashboardConfigFileVersion = "1";

            JsonSerializer serializer = new JsonSerializer();
            serializer.NullValueHandling = NullValueHandling.Ignore;

            using (StreamWriter sw = new StreamWriter(@"dashboard.jsn"))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, dashboardappconf);
            }
        }
    }

}
