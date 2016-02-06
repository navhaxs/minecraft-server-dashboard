using DashboardApp;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;

namespace DashboardApp
{
    
    public class MinecraftServer : System.IDisposable, System.ComponentModel.INotifyPropertyChanged
    {
        private Process _process;

        private DashboardApp.Config.MyUserSettings UserSettings;


        public MinecraftServer()
        {
            // References
            App MyApplication = ((App)Application.Current);
            UserSettings = MyApplication.userSettings;
        }


        //Quote: To start the server with more ram, launch it as "java -Xmx1024M -Xms1024M -jar minecraft_server.jar"
        public bool ReloadStartupParameters()
        {
            _process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = UserSettings.JarExe,
                    WorkingDirectory = UserSettings.WorkingDirectory,

                    Arguments = UserSettings.JREParams + " -jar " + UserSettings.JarFile + " nogui " + UserSettings.MinecraftServerParams,
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


        public void StartServer()
        {
            if (ServerIsOnline)
            {
                ConsoleStream += "[Dashboard] Could not start server, because the server is already running.\r";
            }
            else if (!ReloadStartupParameters())
            {
            }

            try
            {
                _process.Start();
                _process.BeginOutputReadLine(); //Catch console 'log' output
                _process.BeginErrorReadLine();  //CraftBukkit only outputs in the STDERR stream for some reason.

                CurrentServerState = ServerState.WarmUp;

                ConsoleStream += "[Dashboard] Hello! Starting server\r";

                _process.OutputDataReceived += new DataReceivedEventHandler(ServerProc_DataReceived);
                _process.ErrorDataReceived += new DataReceivedEventHandler(ServerProc_DataReceived);
                _process.Exited += new System.EventHandler(ServerProc_Exited);

                // * thing * //
                //var reader = _serverProcess.StandardOutput;
                //while (!reader.EndOfStream)
                //{
                //    // the point is that the stream does not end until the process has 
                //    // finished all of its output.
                //    var nextLine = reader.ReadLine();
                //}

                //_serverProcess.WaitForExit();
            }
            catch (Exception e)
            {
                ConsoleStream += "[Dashboard] Put helpful message here...\r";
                ConsoleStream += e.Message + "\r";
            }
        }

        public void StopServer()
        {
            if (ServerIsOnline)
            {
                CurrentServerState = ServerState.Stopping;
                _process.StandardInput.WriteLine("stop");
            }
        }

        public void KillServer()
        {
            if (ServerIsOnline)
            {
                if (MessageBox.Show("This will force stop the server.", "Warning", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    _process.Kill();
                }
            }
        }

        public bool SendCommand(string command)
        {
            ConsoleStream += ">" + command + "\r";

            if (ServerIsOnline)
            {
                //Only send commands if the server process is running
                _process.StandardInput.Write(command + "\r"); // Write the command into the process, then press 'enter'
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
        }


        /// <summary>
        /// Called when the Minecraft server process exists
        /// </summary>
        private void
        ServerProc_Exited(object sender, System.EventArgs e)
        {
            // Occasionally the .NET framework incorrectly triggers the Exited event

            if (_process.HasExited)
            {
                CurrentServerState = ServerState.NotRunning;

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

        }


        public ServerState CurrentServerState
        {
            get { return _ServerState; }
            set
            {
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