using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using GalaSoft.MvvmLight.Messaging;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System;
using System.Windows.Threading;
using System.Threading;

namespace DashboardApp.Views
{
    /// <summary>
    /// Description for WelcomeView.
    /// </summary>
    public partial class WelcomeView : MetroWindow
    {
        private ProgressDialogController _controller = null;

        /// <summary>
        /// Initializes a new instance of the WelcomeView class.
        /// </summary>
        public WelcomeView()
        {
            InitializeComponent();

            Messenger.Default.Register<string>(this, HandleStatusMessage);
        }

        private void HandleStatusMessage(string msg)
        {
            ShowCustomDialog();
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Cancel any download on window close
            var viewModel = (DashboardApp.ViewModel.WelcomeScreenViewModel)DataContext;
            if (viewModel.CancelDownloadButtonCommand.CanExecute(null))
                viewModel.CancelDownloadButtonCommand.Execute(null);
        }

        public async void EnterDownloadScreen()
        {
            var parentWindow = (MetroWindow)Window.GetWindow(this);
            var setting = new MetroDialogSettings()
            {
                AnimateShow = false,
                AnimateHide = false,
                ColorScheme = MetroDialogColorScheme.Accented
            };
            _controller = await this.ShowProgressAsync("", "Fetching server files....", true, setting);

        }

        //public void Update
        //            _controller.SetProgress((double)downloadProgressChangedEventArgs.ProgressPercentage / 100);


        private async void ShowCustomDialog()
        {
            var dialog = (BaseMetroDialog)this.Resources["CustomDialogTest"];

            await this.ShowMetroDialogAsync(dialog);


        }


        bool _shown;

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            if (_shown)
                return;

            _shown = true;

            // Your code here.
            tabControl.SelectedIndex = 1;

        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            tabControl.SelectedIndex = 1;
            
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            tabControl.SelectedIndex = 3;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            tabControl.SelectedIndex = 2;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

}