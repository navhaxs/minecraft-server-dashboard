using System.Diagnostics;
using DashboardApp.Utils;
using GalaSoft.MvvmLight.Messaging;
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

            // Main app startup events

            // Check for first run
            var welcomeWnd = new WelcomeWindow();
            welcomeWnd.ShowDialog();

            if (DetectJava.FindJavaPath() == null)
            {
                // https://www.java.com/download
                Debug.Print("Java detection failed. Either no java, unexpected java installation, or detection didn't work");
            }

    }

        private void ShowWindow(string obj)
        {
            AboutScreenView x = new AboutScreenView {Owner = this};
            x.ShowDialog();
        }


    }
}