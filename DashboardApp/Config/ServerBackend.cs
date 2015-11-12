using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace DashboardApp.Config
{
    public class MyUserSettings : System.ComponentModel.INotifyPropertyChanged
    {
        public MyUserSettings()
        {
            _JarFile = "minecraft_server.jar";
        }


        private string _JarFile;
        public string JarFile
        {
            get { return _JarFile; }
            set { _JarFile = value; OnPropertyChanged("JarFile"); }
        }

        private string _WorkingDirectory = System.Environment.CurrentDirectory;
        public string WorkingDirectory
        {
            get { return _WorkingDirectory; }
            set { _WorkingDirectory = value; OnPropertyChanged("WorkingDirectory"); }
        }

        private string _JarExe = "Java";
        public string JarExe {
            get { return _JarExe; }
            set { _JarExe = value; OnPropertyChanged("JarExe"); }
        }

        private string _MinecraftServerParams = "";
        public string MinecraftServerParams
        {
            get { return _MinecraftServerParams; }
            set { _MinecraftServerParams = value; OnPropertyChanged("MinecraftServerParams"); }
        }

        private string _JREParams = "";
        public string JREParams
        {
            get { return _JREParams; }
            set { _JREParams = value; OnPropertyChanged("JREParams"); }
        }

        // Create the OnPropertyChanged method to raise the event 
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
