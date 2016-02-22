using System.Diagnostics;
using DashboardApp.Utils;
using GalaSoft.MvvmLight.Messaging;
using DashboardApp.Views;
using System.Windows;

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

            // Main app startup events

            // Check for first run
            App myApplication = ((App)Application.Current);
            var userSettings = myApplication.UserSettings;
            if (!userSettings.JarFileExists())
            {
                var welcomeWnd = new WelcomeView();
                welcomeWnd.ShowDialog();

            }


            if (DetectJava.FindJavaPath() == null)
            {
                // https://www.java.com/download
                Debug.Print("Java detection failed. Either no java, unexpected java installation, or detection didn't work");
            }

        }

        private void header_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // HACK ?
            if (((ViewModel.MainViewModel) (this.DataContext)).OverlayPageIndex != (int)ViewModel.MainViewModel.OverlayPage.None)
            {
                e.Cancel = true;
                ((ViewModel.MainViewModel)(this.DataContext)).CommandCloseOverlay.Execute(this);
            }
        }
    }
}