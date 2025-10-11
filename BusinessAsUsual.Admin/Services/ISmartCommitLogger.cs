namespace BusinessAsUsual.Admin.Services {

    /// <summary>
    /// Smart logger interface
    /// </summary>
    public interface ISmartCommitLogger
    {
        /// <summary>
        /// Method to post a message.
        /// </summary>
        /// <param name="message"></param>
        void Log(string message);
    }
}