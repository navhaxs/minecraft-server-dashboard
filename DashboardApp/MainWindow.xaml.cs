using GalaSoft.MvvmLight.Messaging;
using System.Windows;
using System.Windows.Input;
using System;
using DashboardApp.Views;
using DashboardApp.Views.Config;
using System.Windows.Controls;
using System.Windows.Threading;

namespace DashboardApp
{
    /// <summary>
    /// Description for MainWindow.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Messenger.Default.Register<Message.ShowDialog>(this, ShowWindow);
            Messenger.Default.Register<Message.GotoTab>(this, GotoTab);
            Messenger.Default.Register<Message.GotoConfigPage>(this, GotoConfigPage);
            Messenger.Default.Register<Models.Server.Message.ServerStatusChanged>(this, ServerStatusChanged);
            Messenger.Default.Register<Message.ShowErrorMessage>(this, ShowErrorMessage);
        }

        private void ShowErrorMessage(Message.ShowErrorMessage obj)
        {
            var wnd = new ErrorWindow()
            {
                ErrorTitle = obj.Title,
                Title = "XXX",
                ErrorMessage = obj.Message
            };
            Messenger.Default.Send(new MainWindow.Message.ShowDialog() { window = wnd });
        }

        private void ServerStatusChanged(Models.Server.Message.ServerStatusChanged obj)
        {
            Dispatcher.BeginInvoke(new Action(() => {
                switch (obj.NewState)
                {
                    case ServerState.BindCritical:
                        break;
                    case ServerState.NotRunning:
                        EllipseOnline.Visibility = Visibility.Hidden;
                        EllipseOffline.Visibility = Visibility.Visible;
                        break;
                    case ServerState.WarmUp:
                    case ServerState.Stopping:
                    case ServerState.Reloading:
                    case ServerState.Running:
                    default:
                        EllipseOnline.Visibility = Visibility.Visible;
                        EllipseOffline.Visibility = Visibility.Hidden;
                        break;
                }
            }));
        }

        private void ShowWindow(Message.ShowDialog wnd)
        {
            var x = wnd.window;
            x.Owner = this;
            x.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            x.ShowDialog();
        }

        //int MainWindow_Tab_SelectedIndex; //XXX
        public void GotoTab(Message.GotoTab t)
        {
            //XXX
            return;
        }

        public void GotoConfigPage(Message.GotoConfigPage p)
        {
            //XXX overlay in main window
            var f = new Frame();
            f.Navigate(p.page);
            var m = new Window() { Content = f };
            m.ShowDialog();
            return;
        }

        
        public static class Message
        {

            public class ShowDialog
            {
                public Window window;
            }

            public class GotoTab
            {
                public int index;
            }

            public class GotoConfigPage
            {
                public Page page;
            }

            public class ShowErrorMessage
            {
                public string Title;
                public string Message;
            }
        }
    }

}