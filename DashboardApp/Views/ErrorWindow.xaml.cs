using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DashboardApp.Views
{
    /// <summary>
    /// Interaction logic for ErrorWindow.xaml
    /// </summary>
    public partial class ErrorWindow : Window, INotifyPropertyChanged
    {

        public ErrorWindow()
        {
            InitializeComponent();
        }

        private string title;
        public string ErrorTitle { get { return title; } set { title = value; RaisePropertyChanged("ErrorTitle"); } }

        private string message;
        public string ErrorMessage { get { return message; } set { message = value; RaisePropertyChanged("ErrorMessage"); } }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
