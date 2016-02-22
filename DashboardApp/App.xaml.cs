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
        public MinecraftServer MinecraftServer;

        public Config.ConfigStore UserSettings;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            UserSettings = new Config.ConfigStore();
            MinecraftServer = new MinecraftServer();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            UserSettings.Save();
        }
    }
}
