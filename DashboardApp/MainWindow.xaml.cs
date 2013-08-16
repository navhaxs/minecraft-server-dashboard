using System;
using System.Collections.Generic;
using System.Linq;
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

namespace DashboardApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void CommandTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                App.MyServer.SendCommand(CommandTextBox.Text);
                CommandTextBox.Text = "";
            }
            
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ConsoleTextBlock.DataContext = App.MyServer;
            App.MyServer.StartServer();
            MahApps.Metro.ThemeManager.ChangeTheme(this, MahApps.Metro.ThemeManager.DefaultAccents.First(a => a.Name == "Orange"), MahApps.Metro.Theme.Dark);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MahApps.Metro.ThemeManager.ChangeTheme(this, MahApps.Metro.ThemeManager.DefaultAccents.First(a => a.Name == "Blue"), MahApps.Metro.Theme.Light);
            // Server Running
            // Stop Server
        }

        private void ConsoleTextBlock_TextChanged(object sender, TextChangedEventArgs e)
        {
            ConsoleTextBlock.ScrollToEnd();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Window1 wnd = new Window1();
            wnd.Owner = this;
            wnd.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            wnd.ShowDialog();
        }
    }
}
