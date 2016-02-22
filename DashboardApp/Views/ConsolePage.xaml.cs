using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DashboardApp.Views
{
    /// <summary>
    /// Description for ConsolePage.
    /// </summary>
    public partial class ConsolePage : Page
    {
        /// <summary>
        /// Initializes a new instance of the ConsolePage class.
        /// </summary>
        public ConsolePage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// On 'Enter' keydown, pass the entered command to the server process, and keep history of the past entered commands,
        /// then clear the textbox.
        /// </summary>
        private void CommandTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

                if (CommandTextBox.Text != null || !string.IsNullOrEmpty(CommandTextBox.Text))
                {
                    //Get the last command in the history
                    History_selectedIndex = hsCount - 1;
                    string lastvalue = getCommandHistory(CommandHistoryNavigate.Up);

                    //Set the index counter back to the MAXIMUM (because the item is added to the end)
                    History_selectedIndex = hsCount - 1;
                    // Zero-based index

                    // Don't add this value to the history if it was the same as the last one
                    if (!(lastvalue == CommandTextBox.Text))
                    {
                        commandHistory.Add(CommandTextBox.Text);
                    }

                    //Only send commands if the server is running...
                    App MyApplication = ((App)Application.Current);
                    MinecraftServer Server = MyApplication.MinecraftServer;
                    if (Server.ServerIsOnline == true)
                    {
                        Server.SendCommand(CommandTextBox.Text);

                        CommandTextBox.Text = "";
                        // Clear the textbox
                    }

                    History_selectedIndex = hsCount - 1;
                    //Reset counter back to MAX
                }
                ConsoleTextBlock.ScrollToEnd();
            }
        }


        #region "Command history"
        private List<string> commandHistory = new List<string>();
        private int History_selectedIndex = -1;
        private List<string> playerCommands = new List<string>();
        public int hsCount
        {
            get { return commandHistory.Count; }
        }
        public enum CommandHistoryNavigate
        {
            /// <summary>
            /// Cycle through the command history back one
            /// </summary>
            Up = 1,
            /// <summary>
            /// Cycle through the command history forward one
            /// </summary>
            Down = 0
        }

        /// <summary>
        ///
        /// </summary>
        private string getCommandHistory(CommandHistoryNavigate UpOrDown)
        {
            // If history is empty
            if (hsCount == 0)
            {
                return "";
                // Don't do anything
            }

            //Starts at 0

            if (UpOrDown == CommandHistoryNavigate.Up)
            {
                // Go upwards (backwards)
                History_selectedIndex -= 1;

                if (History_selectedIndex < 0)
                {
                    // At the 'top' (start) of the list, cannot go backwards any further
                    History_selectedIndex = 0;
                    return commandHistory[0];
                }
                else
                {
                    return commandHistory[History_selectedIndex + 1];
                }

            }
            else if (UpOrDown == CommandHistoryNavigate.Down)
            {
                // Go downwards (forwards) in history list

                if (History_selectedIndex + 2 > hsCount)
                {
                    // At the 'bottom' (end) of the list, cannot go forwards any further.
                    return "";
                }
                else
                {
                    History_selectedIndex += 1;
                    return commandHistory[History_selectedIndex];
                }
            }
            return "";
        }

        /// <summary>
        /// Allows the cycling through the command history in the textbox
        /// </summary>
        private void CommandTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    CommandTextBox.Text = getCommandHistory(CommandHistoryNavigate.Up);
                    break; // TODO: might not be correct. Was : Exit Select
                case Key.Down:
                    CommandTextBox.Text = getCommandHistory(CommandHistoryNavigate.Down);
                    break; // TODO: might not be correct. Was : Exit Select
                default:
                    break; // TODO: might not be correct. Was : Exit Select
            }
        }
        #endregion

        private void ConsoleTextBlock_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            //var textBox = (TextBox)sender;
            //if (scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight)
            //??if (textBox.VerticalOffset >= textBox.ExtentHeight ) //textBox.ExtentHeight)
                ConsoleTextBlock.ScrollToEnd();
        }
    }
}
