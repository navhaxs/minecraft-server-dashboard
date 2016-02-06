using DashboardApp.Models;
using GalaSoft.MvvmLight;
using System.Windows.Input;
using MahApps.Metro.Controls.Dialogs;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using System;

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
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}

            App MyApplication = ((App)Application.Current);
            Server = MyApplication.minecraftServer;

            CommandStartServer = new RelayCommand(() => Server.StartServer());
            CommandStopServer = new RelayCommand(() => Server.StopServer());
            CommandReloadServer = new RelayCommand(() => Server.SendCommand("reload")); // vanilla: YES, craftbukit: NO
            CommandForceStopServer = new RelayCommand(() => Server.KillServer());
            //CommandJoinNewPlayers = new RelayCommand()
            //CommandMyServerProperties
            //CommandBackendSetup
            CommandExploreServerDir = new RelayCommand(showInWindowsExplorer);
            //CommandNotepadServerProp
            CommandwwwMincraftWiki = new RelayCommand(gotoWiki);
            CommandwwwMincraftForum = new RelayCommand(gotoForums);
            CommandAboutScreen = new RelayCommand(showAboutView);

            UserSettings = MyApplication.userSettings;
        }

        public MinecraftServer Server { set; get; }
        
        public ICommand CommandStartServer { get; set; }
        public ICommand CommandStopServer { get; set; }
        public ICommand CommandReloadServer { get; set; }
        public ICommand CommandForceStopServer { get; set; }
        public ICommand CommandJoinNewPlayers { get; set; }
        public ICommand CommandMyServerProperties { get; set; }
        public ICommand CommandBackendSetup { get; set; }
        public ICommand CommandExploreServerDir { get; set; }
        public ICommand CommandNotepadServerProp { get; set; }
        public ICommand CommandwwwMincraftWiki { get; set; }
        public ICommand CommandwwwMincraftForum { get; set; }
        public ICommand CommandAboutScreen { get; set; }

        public Config.MyUserSettings UserSettings { get; set; }

        public void showInWindowsExplorer()
        {
            // References
            App MyApplication = ((App)Application.Current);
            var UserSettings = MyApplication.userSettings;

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
    }
}