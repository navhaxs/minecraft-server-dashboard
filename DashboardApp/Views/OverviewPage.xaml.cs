using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DashboardApp.Views
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class OverviewPage : Page
    {
        public OverviewPage()
        {
            InitializeComponent();
            Messenger.Default.Register<Models.Server.Message.ServerStatusChanged>(this, ServerStatusChanged);
        }


        private void ServerStatusChanged(Models.Server.Message.ServerStatusChanged obj)
        {
            Dispatcher.BeginInvoke(new Action(() => {
                switch (obj.NewState)
                {
                    case ServerState.BindCritical:
                        break;
                    case ServerState.NotRunning:
                        lblServerStatus.Content = "Server offline";
                        btnStartStop.Content = "Start Server";
                        lblTipServerOnline.Visibility = Visibility.Hidden;
                        lblTipServerOffline.Visibility = Visibility.Visible;
                        break;
                    case ServerState.WarmUp:
                        lblServerStatus.Content = "Server starting...";
                        btnStartStop.Content = "Stop Server";
                        break;
                    case ServerState.Stopping:
                        lblServerStatus.Content = "Server stopping...";
                        break;
                    case ServerState.Reloading:
                        lblServerStatus.Content = "Server reloading...";
                        break;
                    case ServerState.Running:
                        lblServerStatus.Content = "Server online";
                        lblTipServerOnline.Visibility = Visibility.Visible;
                        lblTipServerOffline.Visibility = Visibility.Hidden;
                        break;
                    default:
                        break;
                }
            }));
        }
    }
}
