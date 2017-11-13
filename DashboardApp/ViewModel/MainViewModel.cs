using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Windows.Input;
using DashboardApp.Views.Config;
using System.Windows;
using DashboardApp.Views;
using DashboardApp.Models;

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
            }
            else
            {
                // Code runs "for real"
                App MyApp = ((App)Application.Current);
                server = MyApp.minecraftServer;
            }
        }

        private Server server;

        public ICommand CommandGotoServerProps => new RelayCommand<object>((x) => {
            //Messenger.Default.Send(new MainWindow.Message.GotoConfigPage() { page = new ConfigServerProp() });
            Messenger.Default.Send(new Backend.Message.OpenServerProperties()); // XXX
        });

        public ICommand CommandGotoConfigBackend => new RelayCommand<object>((x) => {
            Messenger.Default.Send(new MainWindow.Message.GotoConfigPage() { page = new ConfigJarfileBackend() });
        });

        public ICommand CommandGotoJava => new RelayCommand<object>((x) => {
            Messenger.Default.Send(new MainWindow.Message.GotoConfigPage() { page = new ConfigJava() });
        });

        public ICommand CommandGotoAppOptions => new RelayCommand<object>((x) => {
            Messenger.Default.Send(new MainWindow.Message.GotoConfigPage() { page = new ConfigMyApp() });
        });

        public ICommand CommandGotoWorldBackups => new RelayCommand<object>((x) => {
            Messenger.Default.Send(new MainWindow.Message.GotoConfigPage() { page = new ConfigMyWorldBackups() });
        });

        public ICommand CommandOpenServerDir => new RelayCommand<object>((x) => {
            Messenger.Default.Send(new Backend.Message.OpenServerDir());
        });

        public ICommand CommandOpenBackupDir => new RelayCommand<object>((x) => {
        });

        public ICommand CommandShowAboutScreen => new RelayCommand<object>((x) => {
            Messenger.Default.Send(new MainWindow.Message.ShowDialog() { window = new AboutScreenView() });
        });

        public ICommand CommandShowJoinNewPlayers => new RelayCommand<object>((x) => {
            Messenger.Default.Send(new MainWindow.Message.ShowDialog() { window = new JoinNewPlayers() });
        });
        
        // Server Actions
        public ICommand CommandStartServer => new RelayCommand<object>((x) => {
            Messenger.Default.Send(new Server.Message.ServerAction() { action = Server.Message.ServerAction.ServerActionType.Start });
        });

        public ICommand CommandStopServer => new RelayCommand<object>((x) => {
            Messenger.Default.Send(new Server.Message.ServerAction() { action = Server.Message.ServerAction.ServerActionType.Stop });
        });

        public string ExternalIP { get; set; } // xxx update prop

        // Reference to the Server model (e.g. for binding ConsoleStream to XAML)
        public Server Server
        {
            get { return server; }
            set { server = value; }
        }
        
    }
}