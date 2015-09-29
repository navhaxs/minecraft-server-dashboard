using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows;
using System.Windows.Input;
using System.Reflection;
using System.Diagnostics;
using System.Linq;
using GalaSoft.MvvmLight.Messaging;

namespace DashboardApp.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
                version = "1.0.0.0";
                githash = "git xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";
            }
            else
            {
                CommandStartServer = new RelayCommand(() => Server.StartServer());
                CommandStopServer = new RelayCommand(() => Server.StopServer());
                doAboutScreen = new RelayCommand(() => ShowAboutScreen());

                // Version data              
                Assembly assembly = Assembly.GetExecutingAssembly();
                FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
                version = fileVersionInfo.FileVersion;
                githash = fileVersionInfo.ProductVersion;

                // References
                App MyApplication = ((App)Application.Current);
                Server = MyApplication.minecraftServer;
                UserSettings = MyApplication.userSettings;
            }

        }

        public string Java;
        public string Jarfile;
        public string WorkingDirectory;

        public MinecraftServer Server { get; set; }
        public Config.MyUserSettings UserSettings { get; set; }

        public string version { get; set; }
        public string githash { get; set; }

        public ICommand doAboutScreen { get; private set; }
        public ICommand CommandStartServer { get; private set; }
        public ICommand CommandStopServer { get; private set; }
        public ICommand CommandReloadServer { get; private set; }
        public ICommand CommandForceStopServer { get; private set; }

        private void ShowAboutScreen()
        {
            Messenger.Default.Send("Some text");
        }

        private void ShowPopUpExecute()
        {
            MessageBox.Show("Hello!");
        }
    }
}