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

        // Models
        public Models.Server minecraftServer;
        public Models.Backend app;


        /// <summary>
        /// Initialize all models at App startup
        /// </summary>
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            app = new Models.Backend(); 

            minecraftServer = new Models.Server();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            app.AppShutdown();
        }
    }
}
