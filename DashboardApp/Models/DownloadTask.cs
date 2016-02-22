using System;
using System.ComponentModel;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace DashboardApp.Models
{
    public class DownloadTask
    {
        public event StartedHandler DownloadStarted;
        public delegate void StartedHandler(DownloadTask t, EventArgs e);

        public event FilenameReceivedHandler VersionFetchCompleted;
        public delegate void FilenameReceivedHandler(DownloadTask t, EventArgs e);

        public event ProgressChangedHandler ProgressChanged;
        public delegate void ProgressChangedHandler(DownloadTask t, DownloadProgressChangedEventArgs e);

        public event CompletedHandler Completed;
        public delegate void CompletedHandler(DownloadTask t, EventArgs e);

        public EventArgs e = null;

        public class ValueChangedEventArgs : EventArgs
        {
            public int Value;
        }

        public bool IsBusy { get; internal set; }

        public string ver { get; internal set; }

        private WebClient wc;

        public DownloadTask()
        {
            wc = new WebClient();
            wc.DownloadProgressChanged += WcOnDownloadProgressChanged;
            wc.DownloadFileCompleted += WcOnDownloadFileCompleted;
        }

        internal void BeginDownload()
        {
            if (IsBusy != true)
            {
                IsBusy = true;
                Task.Factory.StartNew(DoWork, TaskCreationOptions.AttachedToParent); 
            }
        }

        private void DoWork()
        {
            // Dashboard uses the current directory as the working directory
            // for simplicity.
            var rootDir = System.Environment.CurrentDirectory;
            var url = "https://s3.amazonaws.com/Minecraft.Download/versions/" + ver + "/minecraft_server." + ver + ".jar";
            //var url = "http://navhaxs.au.eu.org/foobar.txt";

            wc.DownloadFileAsync(new Uri(url), rootDir + "/minecraft_server." + ver + ".jar");

            System.Diagnostics.Debug.Print("Starting download of " + "/minecraft_server." + ver + ".jar");
            if (DownloadStarted != null) DownloadStarted(this, e); // Raise event
        }

        /// <summary>
        /// Fetch from the internet what is the latest server version
        /// </summary>
        internal void BeginVersionFetch()
        {
            if (IsBusy != true)
            {
                IsBusy = true;
                Task.Factory.StartNew(doBeginVersionFetch, TaskCreationOptions.AttachedToParent);
            }
        }

        /// <summary>
        /// Run the version fetch in a background task
        /// </summary>
        private void doBeginVersionFetch()
        {
            ver = Utils.ServerJarfile.GetLatesturl();
            IsBusy = false;
            // Wait until we get the online data
            if (VersionFetchCompleted != null) VersionFetchCompleted(this, e); // Raise event
        }

        internal void CancelDownload()
        {
            try
            {
                if (IsBusy) // cancel if download task is running
                {
                    System.Diagnostics.Debug.Print("Cancelled download!");
                    wc.CancelAsync();
                }
            }
            catch (Exception e)
            {
                // TODO Logger
                //
            }
        }

        /// <summary>
        /// Trigger the file download completed event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="asyncCompletedEventArgs"></param>
        private void WcOnDownloadFileCompleted(object sender, AsyncCompletedEventArgs asyncCompletedEventArgs)
        {
            System.Diagnostics.Debug.Print("Download finished.");
            IsBusy = false;
            if (Completed != null)
            {
                Completed(this, e); // Raise event
            }
        }

        /// <summary>
        /// Trigger an update of the download progress bar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="downloadProgressChangedEventArgs"></param>
        private void WcOnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs downloadProgressChangedEventArgs)
        {
            System.Diagnostics.Debug.Print(downloadProgressChangedEventArgs.ProgressPercentage.ToString());
            if (ProgressChanged != null) ProgressChanged(this, downloadProgressChangedEventArgs); // Raise event
        }
    }
}