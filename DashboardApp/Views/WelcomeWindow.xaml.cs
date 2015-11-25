using MahApps.Metro.Controls;

/* Currently not using MVVM pattern for navigation between the Welcome screens.

    Using the NavigationService to jump between the wizard pages:
    NavigationService.Navigate(new Uri("Views/Welcome/NewServerPage.xaml", UriKind.Relative));
    See: https://msdn.microsoft.com/en-us/library/aa349685(v=vs.100).aspx
  
    25/11/2015
 */

namespace DashboardApp.Views
{
    /// <summary>
    /// Interaction logic for WelcomeWindow.xaml
    /// </summary>
    public partial class WelcomeWindow : MetroWindow
    {
        public WelcomeWindow()
        {
            InitializeComponent();
        }
    }
}
