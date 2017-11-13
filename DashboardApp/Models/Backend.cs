using GalaSoft.MvvmLight.Messaging;
using System.ComponentModel;
using System;
using GalaSoft.MvvmLight;

namespace DashboardApp.Models
{
    /// <summary>
    /// This model contains the core logic.
    /// </summary>
    public class Backend : ViewModelBase // XXX less ambigous name
    {
    
        // Initialise models
        public Config config;
        public ServerProperties ServerProps;

        public Backend()
        {
            config = new Config();
            ServerProps = new ServerProperties();

            Messenger.Default.Register<Message.OpenServerDir>(this, OpenServerDir);
            Messenger.Default.Register<Message.OpenServerProperties>(this, OpenServerProperties);

        }

        private void OpenServerProperties(Message.OpenServerProperties obj)
        {
            System.Diagnostics.Process.Start("notepad.exe", WorkingDirectory + "\\server.properties");
        }

        private void OpenServerDir(Message.OpenServerDir obj)
        {
            // Should always succeed if Server Path is System.Env.Path ...
            if (System.IO.Directory.Exists(WorkingDirectory))
            {
                System.Diagnostics.Process.Start("explorer.exe", WorkingDirectory);
            }
            else
            {
                //MessageWindow(MyMainWindow, "", "Please configure the server backend first.")
            }
        }

        public void AppShutdown()
        {
            config.Save();
        }

        public string JarExe { get { return config.dashboardappconf.Startup_JavaExec; } set { config.dashboardappconf.Startup_JavaExec = value; RaisePropertyChanged(""); } }
        public string JREParams { get { return config.dashboardappconf.Startup_JavaSpecificArgs; } set { config.dashboardappconf.Startup_JavaSpecificArgs = value; RaisePropertyChanged(""); } }

        public string WorkingDirectory { get { return System.Environment.CurrentDirectory; } } // XXX don't make user settable
        public string JarFile { get { return config.dashboardappconf.Jarfile; } set { config.dashboardappconf.Jarfile = value; RaisePropertyChanged(""); } }
        public string MinecraftServerParams { get { return config.dashboardappconf.JarLaunchArguments; } set {
                config.dashboardappconf.JarLaunchArguments = value; RaisePropertyChanged(""); } }
        public bool RestartRequired { get { return config.RestartRequired; } set { config.RestartRequired = value; RaisePropertyChanged("RestartRequired"); } }


        public class Message
        {
            public class OpenServerDir { }

            internal class OpenServerProperties
            {
                public OpenServerProperties()
                {
                }
            }
        }
    }
}