using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace DashboardApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public MinecraftServer minecraftServer;

        public Config.MyUserSettings userSettings;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            userSettings = new Config.MyUserSettings(); // The server class requires the user settings store to be initialized first.
            minecraftServer = new MinecraftServer();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            Config.ConfigStore.Save();
        }
    }
}
