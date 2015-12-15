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

        private DownloadTask _downloadTask = new DownloadTask();
        private string versionValue = String.Empty;
        private int SelectedIndexValue;
        private int _DownloadProgressValue;

        /// <summary>
        /// Initializes a new instance of the WelcomeScreenViewModel class.
        /// </summary>
        public WelcomeScreenViewModel()
        {
            DownloadButtonCommand = new RelayCommand(BeginDownload);
            CancelDownloadButtonCommand = new RelayCommand(CancelDownload);

            _downloadTask.VersionFetchCompleted += _downloadTask_VersionFetchCompleted;
            _downloadTask.ProgressChanged += _downloadTask_ProgressChanged;
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

        private ICommand _downloadButtonCommand;
        public ICommand DownloadButtonCommand
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

        private void BeginDownload()
        {
            _downloadTask.BeginVersionFetch();   
        }

        private void CancelDownload()
        {
            _downloadTask.CancelDownload();
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
    }
}