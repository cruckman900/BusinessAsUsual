namespace BusinessAsUsual.Admin.Services
{
    /// <summary>
    /// Smart commit Logger implementation
    /// </summary>
    public class SmartCommitLogger : ISmartCommitLogger
    {
        private readonly string _logPath = Path.Combine("Logs", "smart-commits.txt");

        /// <summary>
        /// Flush commit log messages to file.
        /// </summary>
        /// <param name="message"></param>
        public void Log(string message)
        {
            Directory.CreateDirectory("Logs");
            File.AppendAllText(_logPath, $"{message}{Environment.NewLine}");
        }
    }
}
