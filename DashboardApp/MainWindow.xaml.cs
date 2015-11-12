using GalaSoft.MvvmLight.Messaging;
using System.Windows;
using System.Windows.Input;
using System;
using DashboardApp.Views;

namespace DashboardApp
{
    /// <summary>
    /// Description for MainWindow.
    /// </summary>
    public partial class MainWindow : MahApps.Metro.Controls.MetroWindow
    {
        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Messenger.Default.Register<string>(this, ShowWindow);

            //App MyApplication = ((App)Application.Current);
            //BindMeToServerClass.DataContext = MyApplication.Server;
        }

        private void ShowWindow(string obj)
        {
            AboutScreenView x = new AboutScreenView();
            x.Owner = this;
            x.ShowDialog();
        }
    }
}