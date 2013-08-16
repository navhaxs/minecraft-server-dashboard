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

        public static ServerClass MyServer = new ServerClass();

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            MyServer.Dispose();
        }
    }
}
