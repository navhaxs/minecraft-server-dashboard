using System.ComponentModel;

namespace DashboardApp.Config
{
    public class MyUserSettings : INotifyPropertyChanged
    {
        public MyUserSettings()
        {
            // TODO refractor
            ConfigStore.LoadAll();

        }

        public string JarFile
        {
            get { return ConfigStore.settingsStore.Jarfile; }
            set { ConfigStore.settingsStore.Jarfile = value; System.Diagnostics.Debug.Print("JarFile [set]: " + value); OnPropertyChanged("JarFile"); }
        }

        public bool JarFileExists()
        {
            System.Diagnostics.Debug.Print("JarFile: " + JarFile);
            return System.IO.File.Exists(WorkingDirectory + @"\" + JarFile);
        }

        private string _workingDirectory = System.Environment.CurrentDirectory;
        public string WorkingDirectory
        {
            get { return _workingDirectory; }
            set { _workingDirectory = value; OnPropertyChanged("WorkingDirectory"); }
        }

        private string _jarExe = "Java";
        public string JarExe {
            get { return _jarExe; }
            set { _jarExe = value; OnPropertyChanged("JarExe"); }
        }

        private string _minecraftServerParams = "";
        public string MinecraftServerParams
        {
            get { return _minecraftServerParams; }
            set { _minecraftServerParams = value; OnPropertyChanged("MinecraftServerParams"); }
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
