using System;
using DashboardApp.Models;
using Elysium.Controls;
using GalaSoft.MvvmLight;
using MahApps.Metro.Controls;

using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using System.Diagnostics;
using GalaSoft.MvvmLight.Messaging;

using System.ComponentModel;
using System.Windows.Navigation;
using System.Runtime.CompilerServices;
using System.Net;
using System.Collections.Generic;
using System.Windows;

namespace DashboardApp.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class WelcomeScreenViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private JarSelector _jarSelector;
        private DownloadTask _downloadTask;
        private string versionValue = String.Empty;
        private int SelectedIndexValue;
        private int _DownloadProgressValue;

        /// <summary>
        /// Initializes a new instance of the WelcomeScreenViewModel class.
        /// </summary>
        public WelcomeScreenViewModel()
        {
            CommandExploreServerDir = new RelayCommand(showInWindowsExplorer);
            NavDownloadButtonCommand = new RelayCommand(BeginDownload);
            NavExistingButtonCommand = new RelayCommand(GotoExisting);
            CancelDownloadButtonCommand = new RelayCommand(CancelDownload);
            ConfirmJarfileCommand = new RelayCommand(ConfirmJarfile);

            _downloadTask = new DownloadTask();
            _downloadTask.VersionFetchCompleted += _downloadTask_VersionFetchCompleted;
            _downloadTask.ProgressChanged += _downloadTask_ProgressChanged;
            _downloadTask.Completed += _downloadTask_Completed;

            _jarSelector = new JarSelector();
        }

        private void _downloadTask_Completed(DownloadTask t, EventArgs e)
        {
            SelectedIndex += 1; // finished downloading - next ui page!
        }

        private void _downloadTask_ProgressChanged(DownloadTask t, DownloadProgressChangedEventArgs e)
        {
            DownloadProgressValue = e.ProgressPercentage;
        }

        private void _downloadTask_VersionFetchCompleted(DownloadTask t, EventArgs e)
        {
            Version = t.ver;
            SelectedIndex += 1; // begin downloading - next ui page!
            _downloadTask.BeginDownload();
        }

        public string SelectedValue { get; set; }

        private string _SingleJarFilename;
        public string SingleJarFilename
        {
            get
            {
                return this._SingleJarFilename;
            }

            set
            {
                if (value != this._SingleJarFilename)
                {
                    this._SingleJarFilename = value;
                    NotifyPropertyChanged();
                }
            }
        }


        public ICommand ConfirmJarfileCommand { get; set; }

        public ICommand CommandExploreServerDir { get; set; }

        public bool IsNotSingleJarfile { get; set; }
        public bool IsSingleJarfile { get; set; }

        private ICommand _navExistingButtonCommand;
        public ICommand NavExistingButtonCommand
        {
            get
            {
                return _navExistingButtonCommand;
            }
            set
            {
                _navExistingButtonCommand = value;
            }
        }


        private ICommand _downloadButtonCommand;
        public ICommand NavDownloadButtonCommand
        {
            get
            {
                return _downloadButtonCommand;
            }
            set
            {
                _downloadButtonCommand = value;
            }
        }

        //CancelDownloadButtonCommand
        private ICommand _CancelDownloadButtonCommand;
        public ICommand CancelDownloadButtonCommand
        {
            get
            {
                return _CancelDownloadButtonCommand;
            }
            set
            {
                _CancelDownloadButtonCommand = value;
            }
        }

        private void GotoExisting()
        {
            // Scan the directory for Jar files.
            _jarSelector.Search();

            // If no jar files exist
            if (_jarSelector.IsJarAvailable() != true)
            {
                // Ask the user to move the Dashboard exe to the same folder as the jar files
                SelectedIndex = 5;
            } else
            {
                // Else show the jar file selection screen.
                SelectedIndex = 6;
            }
        }

        private void BeginDownload()
        {
            _downloadTask.BeginVersionFetch();   
        }

        private void CancelDownload()
        {
            _downloadTask.CancelDownload();
        }

        //BooleanToVisibilityConverter
        //IsSingleJarfile

        private List<string> _jarfileList;
        public List<string> JarfileList
        {
            get
            {
                _jarfileList = new List<string>();
                
                // TODO: move to function (ICommand)
                _jarSelector.Search();
                _jarfileList = _jarSelector.GetList();
                if (_jarfileList.Count == 1)
                {
                    IsSingleJarfile = true;
                    IsNotSingleJarfile = false;
                } else {
                    IsSingleJarfile = false;
                    IsNotSingleJarfile = true;
                }

                PropertyChanged(this, new PropertyChangedEventArgs("IsSingleJarfile"));
                PropertyChanged(this, new PropertyChangedEventArgs("IsNotSingleJarfile"));

                return _jarfileList;
            }
        }

        

        public string Version
        {
            get
            {
                return this.versionValue;
            }

            set
            {
                if (value != this.versionValue)
                {
                    this.versionValue = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool DownloadProgressIsIndeterminate
        {
            get
            { return false;
            }
        }

        public int DownloadProgressValue
        {
            get
            {
                return this._DownloadProgressValue; // todo: reset to zero on download cancel
            }

            set
            {
                if (value != this._DownloadProgressValue)
                {
                    this._DownloadProgressValue = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public int SelectedIndex
        {
            get
            {
                return this.SelectedIndexValue;
            }

            set
            {
                if (value != this.SelectedIndexValue)
                {
                    this.SelectedIndexValue = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        // TODO redundancy
        public void showInWindowsExplorer()
        {
            // References
            App MyApplication = ((App)Application.Current);
            var UserSettings = MyApplication.userSettings;

            System.Diagnostics.Process.Start("explorer.exe", UserSettings.WorkingDirectory);
        }

        public void ConfirmJarfile()
        {
            App MyApplication = ((App)Application.Current);
            var UserSettings = MyApplication.userSettings;

            UserSettings.JarFile = SelectedValue;
        }
    }
}