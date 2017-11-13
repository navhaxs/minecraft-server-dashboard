using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using DashboardApp.Utils;
using DashboardApp.Models;

namespace DashboardApp.ViewModel
{
    public class BackendConfigViewModel : ViewModelBase
    {

        /// <summary>
        /// Initializes a new instance of the BackendConfigViewModel class.
        /// </summary>
        public BackendConfigViewModel()
        {
            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
            }
            else
            {
                // Code runs "for real"
                App MyApp = ((App)System.Windows.Application.Current);
                Dashboard = MyApp.app;
                Server = MyApp.minecraftServer;
            }
        }

        public Server Server { get; set; }

        public Backend Dashboard { get; set; }

        public ObservableCollection<string> JarfileList => new ObservableCollection<string>(BackendConfig.GetListofJarfile());

        // XXX config
        public string SelectedJarfile
        {
            get
            {
                return Dashboard.JarFile; //XXX filename only
            }
            set
            {
                Dashboard.JarFile = value;
            }
        }

        // XXX 
        public string FoundJavaVersionString
        {
            get
            {
                return "JRE here";
            }
        }

        // IP address
        public string ExternalIP
        {
            get { return Utils.GetIP.GetExternalIP(); }
        }

        public string ExternalPortNumber
        {
            get { return Utils.GetIP.GetExternalIP(); } // XXX
        }

    }
}

