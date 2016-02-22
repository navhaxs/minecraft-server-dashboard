using DashboardApp;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;

//TODO: To start the server with more ram, launch it as "java -Xmx1024M -Xms1024M -jar minecraft_server.jar"


namespace DashboardApp
{
    
    public class MinecraftServer : System.IDisposable, System.ComponentModel.INotifyPropertyChanged
    {
        
        public MinecraftServer()
        {
            // get the user settings singleton
            App MyApplication = ((App)Application.Current);
            UserSettings = MyApplication.UserSettings;
        }

        // the java.exe process
        private Process _process;

        // enforce commands to be run one at a time (e.g. StartServer(), StopServer(), etc.)
        private Object thisLock = new Object();

        // internal reference
        private DashboardApp.Config.ConfigStore UserSettings;

        /// <summary>
        /// Start the Minecraft game server.
        /// </summary>
        public void StartServer()
        {
            if (ServerIsOnline)
            {
                // TODO: log me. assert failed? who called this func?
                ConsoleStream += "[Dashboard] Could not start server, because the server is already running.\r";
                return;
            }
            else if (!ReloadStartupParameters())
            {
                ConsoleStream += "[Dashboard] Dashboard needs to be set up before starting the server!\r";
                ConsoleStream += "[Dashboard] Please goto the Backend Set-up screen.\r";
                return;
            }

            lock (thisLock)
            {
                try
                {
                     _process.Start();
                    CurrentServerState = ServerState.WarmUp;
                    ConsoleStream += "[Dashboard] Starting up server.\r";

                    // redirect all java output
                    // also redirect stderror, as CraftBukkit only outputs in this stream for some reason
                    _process.BeginOutputReadLine(); 
                    _process.BeginErrorReadLine();

                    // now that streams are redirected, attach event handlers
                    _process.OutputDataReceived += new DataReceivedEventHandler(ServerProc_DataReceived);
                    _process.ErrorDataReceived += new DataReceivedEventHandler(ServerProc_DataReceived);
                    _process.Exited += new System.EventHandler(ServerProc_Exited);


                }
                catch (Exception e)
                {
                    ConsoleStream += "[Dashboard] Problem starting the server :(\r";
                    ConsoleStream += "[Dashboard] Please check the Java path and Jarfile path in Backend Set-up.\r";
                    ConsoleStream += "Exception.Message: " + e.Message + "\r";
                    if (e.InnerException != null) ConsoleStream += "InnerException.Message: " + e.InnerException.Message + "\r"; ;
                }
            }

        }

        /// <summary>
        /// Set up the Process with user settings.
        /// </summary>
        /// <returns>False if any file paths failed validation, else True</returns>
        private bool ReloadStartupParameters()
        {
            if (!UserSettings.JarFileExists()) // error checking
            {
                return false;
            }
            if (!System.IO.File.Exists(UserSettings.SettingsJson.JavaJRE))
            {
                // if not full path - how to check this condition? PATH?

            }
            if (!System.IO.File.Exists(UserSettings.SettingsJson.Jarfile))
            {
                // if not full path - how to check this condition? PATH?
            }

            // instantiate a new Process object with the user's parameters
            _process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = UserSettings.SettingsJson.JavaJRE,
                    WorkingDirectory = UserSettings.WorkingDirectory,

                    Arguments = UserSettings.SettingsJson.JavaArguments + " -jar " + UserSettings.SettingsJson.Jarfile + " nogui " + UserSettings.SettingsJson.JarfileArguments,
                    RedirectStandardInput = true,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            _process.EnableRaisingEvents = true;
            return true;
        }

        public void StopServer()
        {
            if (ServerIsOnline)
            {
                lock (thisLock)
                {
                    CurrentServerState = ServerState.Stopping;

                    try
                    {
                        _process.StandardInput.WriteLine("stop");
                    } catch (Exception e)
                    {
                        KillServer();
                    }

                }
            }
        }

        public void KillServer()
        {
            if (ServerIsOnline)
            {
                if (MessageBox.Show("This will force stop the server.", "Warning", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    lock (thisLock)
                    {
                        _process.Kill();
                    }
                }
            }
        }

        public bool SendCommand(string command)
        {
            ConsoleStream += ">" + command + "\r";

            if (ServerIsOnline)
            {
                //Only send commands if the server process is running
                lock (thisLock)
                {
                    _process.StandardInput.Write(command + "\r"); // Write the command into the process, then press 'enter'
                }
                return true;
            }
            else
            {
                //Do nothing if the server is NOT running
                ConsoleStream += "[Dashboard] The server is not currently running. (Failed to send command '" + command + "')\r";
                return false;
            }
        }

        private void ServerProc_DataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null)
            {
                return;
            }
            ConsoleStream += e.Data + "\r";

            //TODO: fork thread to process for events? (by regex)

            Regex regex = new Regex(@"]: Done\((\d+).(\d+)s\)!");
            Match match = regex.Match(e.Data);
            if (match.Success)
            {
                lock (thisLock)
                {
                    CurrentServerState = ServerState.Running;

                }
            }
            //[Server thread/INFO]: Done(10.032s)! For help, type "help" or "?"
        }


        /// <summary>
        /// Called when the Minecraft server process exists
        /// </summary>
        private void ServerProc_Exited(object sender, System.EventArgs e)
        {
            CurrentServerState = ServerState.NotRunning;

            //lock (thisLock)
            //{
                if (_process.HasExited)
                {


                    string exitmssg = null;
                    if (_process.ExitCode == 0)
                    {
                        exitmssg = "The server stopped successfully.";
                    }
                    else
                    {
                        exitmssg = "The server stopped with an error code of " + _process.ExitCode;
                    }
                    ConsoleStream += "[Dashboard] " + exitmssg + "\r";
                }
            //}
        }

        // lock?
        public ServerState CurrentServerState
        {
            get { return _ServerState; }
            set
            {
                // broadcast state change to any subscribed view models
                var msg = new Models.ServerStateChangeMessage() { newState = value };
                Messenger.Default.Send<Models.ServerStateChangeMessage>(msg);
                
                _ServerState = value;
                //ServerStateChanged(value);
                //OnPropertyChanged("ServerIsOnline");
            }
        }


        private string _consolestream; // This variable holds the entire output stream of the server process.
                                       // TODO: better way?

        public string ConsoleStream
        {
            get { return _consolestream; }
            set
            {
                _consolestream = value;
                OnPropertyChanged("ConsoleStream");
            }
        }



        /// <summary>
        /// Is the server process running?
        /// </summary>
        public bool ServerIsOnline
        {
            get
            {
                if (CurrentServerState == ServerState.NotRunning)
                {
                    return false;
                }
                else
                {
                    //
                    if (_process == null)
                    {
                        MessageBox.Show("FAIL");
                        CurrentServerState = ServerState.NotRunning;
                        return false;
                    }
                    else {
                        //
                        return true;
                    }
                }
            }
        }

        // Encapsulated variable
        private ServerState _ServerState = ServerState.NotRunning;


        //public delegate void ServerStateChangedEventHandler(ServerState newstate);
        //public event ServerStateChangedEventHandler ServerStateChanged;

        public event PropertyChangedEventHandler PropertyChanged;
        // Create the OnPropertyChanged method to raise the event 
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public void Dispose()
        {
            Dispose(true);
            //GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_process != null)
                {
                    try
                    {
                        if (_process.HasExited != true) _process.Kill();
                    }
                    catch
                    {

                    }
                }
            }
        }

    }

}