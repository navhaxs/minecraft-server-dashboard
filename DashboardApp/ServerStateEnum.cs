using System.ComponentModel;

namespace DashboardApp
{
    /// <summary>
    /// The state of the server process
    /// </summary>
    public enum ServerState
    {
        /// <summary>
        /// Running and accepting connections
        /// </summary>
        [Description("Running")]
        Running = 1,
        /// <summary>
        /// Craftbukkit is not running
        /// </summary>
        [Description("NotRunning")]
        NotRunning = 0,
        /// <summary>
        /// Starting up
        /// </summary>
        [Description("Startup")]
        WarmUp = -1,
        /// <summary>
        /// Stopping
        /// </summary>
        [Description("Stopping")]
        Stopping = -2,
        /// <summary>
        /// Server is reloading
        /// </summary>
        [Description("Reloading")]
        Reloading = 3,
        /// <summary>
        /// Server has deteced a critical error
        /// </summary>
        [Description("Critical Error")]
        BindCritical = -9
    }

}
