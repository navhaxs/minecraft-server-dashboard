using DashboardApp.Models;
using GalaSoft.MvvmLight;
using System.Windows.Input;
using MahApps.Metro.Controls.Dialogs;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Messaging;
using System.ComponentModel;
using System.Runtime.CompilerServices;

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
    public class MainViewModel : ViewModelBase, INotifyPropertyChanged
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}

            App MyApplication = ((App)Application.Current);
            Server = MyApplication.MinecraftServer;

            CommandStartServer = new RelayCommand(() => Server.StartServer());
            CommandStopServer = new RelayCommand(() => Server.StopServer());
            CommandReloadServer = new RelayCommand(() => Server.SendCommand("reload")); // vanilla: YES, craftbukit: NO
            CommandForceStopServer = new RelayCommand(() => Server.KillServer());
            CommandJoinNewPlayers = new RelayCommand(() => MessageBox.Show("Not implemented"));
            CommandGotoPropEditor = new RelayCommand(() => SetOverlay(OverlayPage.PropEditor));
            CommandGotoJarConf = new RelayCommand(() => SetOverlay(OverlayPage.JarConf));
            CommandGotoBackendConf = new RelayCommand(() => SetOverlay(OverlayPage.JavaRuntimeConf));
            CommandGotoDashOptions = new RelayCommand(() => SetOverlay(OverlayPage.AppOptions));
            CommandDoBackup = new RelayCommand(() => MessageBox.Show("Not implemented"));
            CommandExploreServerDir = new RelayCommand(showInWindowsExplorer);
            CommandNotepadServerProp = new RelayCommand(() => MessageBox.Show("Not implemented"));
            CommandwwwMincraftWiki = new RelayCommand(gotoWiki);
            CommandwwwMincraftForum = new RelayCommand(gotoForums);
            CommandAboutScreen = new RelayCommand(showAboutView);
            CommandCloseOverlay = new RelayCommand(ClearOverlay);

            UserSettings = MyApplication.UserSettings;

            Messenger.Default.Register<Models.ServerStateChangeMessage>
            (
                 this,
                 (action) => ReceiveStateChangeMessage(action)
            );

            // set default server state
            var msg = new Models.ServerStateChangeMessage() { newState = ServerState.NotRunning };
            ReceiveStateChangeMessage(msg);

            // set default overlay state
            ClearOverlay();

            // foo
            Jarfiles = new ObservableCollection<string>();
        }

        // reference to the server object
        // public to allow access to ConsoleStream
        public MinecraftServer Server { set; get; }

        // Server commands
        public ICommand CommandStartServer { get; set; }
        public ICommand CommandStopServer { get; set; }
        public ICommand CommandReloadServer { get; set; }
        public ICommand CommandForceStopServer { get; set; }
        public ICommand CommandJoinNewPlayers { get; set; }

        // Menu actions
        public ICommand CommandDoBackup { get; set; }
        public ICommand CommandExploreServerDir { get; set; }
        public ICommand  CommandExploreBackupDir { get; set; }
        public ICommand CommandNotepadServerProp { get; set; }
        public ICommand CommandwwwMincraftWiki { get; set; }
        public ICommand CommandwwwMincraftForum { get; set; }

        // UI
        public ICommand CommandGotoPropEditor { get; set; }
        public ICommand CommandGotoJarConf { get; set; }
        public ICommand CommandGotoBackendConf { get; set; }
        public ICommand CommandGotoDashOptions { get; set; }

        public ICommand CommandCloseOverlay { get; set; }

        public ICommand CommandAboutScreen { get; set; }

        public ICommand CommandRefreshJarfiles { get; set; }


        public Config.ConfigStore UserSettings { get; set; }

        public void showInWindowsExplorer()
        {
            // References
            App MyApplication = ((App)Application.Current);
            var UserSettings = MyApplication.UserSettings;

            System.Diagnostics.Process.Start("explorer.exe", UserSettings.WorkingDirectory);

        }

        public void showAboutView()
        {
            var m = new Views.AboutScreenView();
            m.ShowDialog();
        }

        public void gotoWiki()
        {
            System.Diagnostics.Process.Start("http://MinecraftWiki.net");
        }

        public void gotoForums()
        {
            System.Diagnostics.Process.Start("http://MinecraftForum.net");
        }

        public ObservableCollection<string> Jarfiles { get; set; }

        #region "ui: server state"
        // UI strings: current server state
        public string HeaderBarServerStatusText { get; set; } // Top right corner
        public string OverviewServerStatusText { get; set; } // Overview hero text

        // UI element visibility: Visible if server offline, else hidden.
        public Visibility OfflineState_Visibility { get; set; }

        // UI element visibility: Hidden if server offline, else visible.
        public Visibility OnlineState_Visibility { get; set; }

        //
        public Visibility Is_infotip_RestartRequired { get { return Visibility.Collapsed; } }

        private object ReceiveStateChangeMessage(Models.ServerStateChangeMessage action)
        {
            bool serverOnline = true;
            switch (action.newState)
            {
                case ServerState.Running:
                    HeaderBarServerStatusText = "Online";
                    break;
                case ServerState.NotRunning:
                    serverOnline = false;
                    HeaderBarServerStatusText = "Offline";
                    break;
                case ServerState.WarmUp:
                    HeaderBarServerStatusText = "Starting";
                    break;
                case ServerState.Stopping:
                    HeaderBarServerStatusText = "Stopping";
                    break;
                case ServerState.Reloading:
                    HeaderBarServerStatusText = "Online";
                    break;
                case ServerState.NotRunningBindCritical: // TODO 
                    serverOnline = false;
                    HeaderBarServerStatusText = "Error";
                    break;
                default:
                    break;
            }

            OnlineState_Visibility = (serverOnline) ? Visibility.Visible : Visibility.Collapsed;
            OfflineState_Visibility = (serverOnline) ? Visibility.Collapsed : Visibility.Visible;

            OverviewServerStatusText = "Server " + HeaderBarServerStatusText; // TODO localize

            NotifyPropertyChanged("OfflineState_Visibility");
            NotifyPropertyChanged("OnlineState_Visibility");
            NotifyPropertyChanged("HeaderBarServerStatusText");
            NotifyPropertyChanged("OverviewServerStatusText");



            return null;
        }

        #endregion

        #region "overlay"
        public int OverlayPageIndex { get; set; }

        public void SetOverlay(OverlayPage page)
        {
            MainOverlay_IsNotEnabled = false;
            MainOverlay_Visibility = Visibility.Visible;
            OverlayPageIndex = (int) page;
            NotifyPropertyChanged("MainOverlay_IsNotEnabled");
            NotifyPropertyChanged("MainOverlay_Visibility");
            NotifyPropertyChanged("OverlayPageIndex");
        }

        public enum OverlayPage
        {
            None = 0,
            PropEditor = 1,
            JarConf = 2,
            JavaRuntimeConf = 3,
            AppOptions = 4
        }

        public void ClearOverlay()
        {
            MainOverlay_IsNotEnabled = true;
            MainOverlay_Visibility = Visibility.Collapsed;
            OverlayPageIndex = (int)OverlayPage.None;
            NotifyPropertyChanged("MainOverlay_IsNotEnabled");
            NotifyPropertyChanged("MainOverlay_Visibility");
            NotifyPropertyChanged("OverlayPageIndex");
        }

        public bool MainOverlay_IsNotEnabled { get; set; }
        public Visibility MainOverlay_Visibility { get; set; }
        #endregion

        #region "INotifyPropertyChanged boilerplate code"

            public event PropertyChangedEventHandler PropertyChanged;
                
        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}