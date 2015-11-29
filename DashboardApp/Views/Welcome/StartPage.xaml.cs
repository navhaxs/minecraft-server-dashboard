using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace DashboardApp.Views.Welcome
{
    /// <summary>
    /// Interaction logic for StartPage.xaml
    /// </summary>
    public partial class StartPage : Page
    {
        public StartPage()
        {
            InitializeComponent();
        }

        private ProgressDialogController _controller = null;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke((Action)(async () =>
            {
                var parentWindow = (MetroWindow)Window.GetWindow(this);
                var setting = new MetroDialogSettings()
                {
                    AnimateShow = false,
                    AnimateHide = false,
                    ColorScheme = MetroDialogColorScheme.Accented
                };
                _controller = await parentWindow.ShowProgressAsync("", "Fetching server files....", true, setting);

                DownloadServerFiles();
            }));
             
         
        }

        private void DownloadServerFiles()
        {
            var wc = new WebClient();
            var ver = Utils.ServerJarfile.GetLatesturl();
            var url = "https://s3.amazonaws.com/Minecraft.Download/versions/" + ver + "/minecraft_server." + ver + ".jar";
            var rootDir = "C:\\Tmp";
            wc.DownloadProgressChanged += WcOnDownloadProgressChanged;
            wc.DownloadFileCompleted += WcOnDownloadFileCompleted;

            wc.DownloadFileAsync(new Uri(url), rootDir + "\\random.jar");
        }

        private void WcOnDownloadFileCompleted(object sender, AsyncCompletedEventArgs asyncCompletedEventArgs)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                _controller.CloseAsync();
            }));
        }

        private void WcOnDownloadProgressChanged(object sender,
            DownloadProgressChangedEventArgs downloadProgressChangedEventArgs)
        {
            Dispatcher.BeginInvoke((Action) (() =>
            {
                _controller.SetProgress((double) downloadProgressChangedEventArgs.ProgressPercentage / 100);
            }));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("Views/Welcome/ExistingServerSelectionPage.xaml", UriKind.Relative));
        }
    }
}
