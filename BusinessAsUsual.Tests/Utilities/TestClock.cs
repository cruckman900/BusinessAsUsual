namespace BusinessAsUsual.Tests.Utilities
{
    /// <summary>
    /// Provides a controllable clock for testing time-dependent logic.
    /// </summary>
    public class TestClock
    {
        private DateTime? _frozenTime;

        /// <summary>
        /// Gets the current time. If frozen, returns the frozen value.
        /// </summary>
        public DateTime Now => _frozenTime ?? DateTime.UtcNow;

        /// <summary>
        /// Freezes the clock at the specified time.
        /// </summary>
        public void Freeze(DateTime time) => _frozenTime = time;

        /// <summary>
        /// Unfreezes the clock, returning to real time.
        /// </summary>
        public void Unfreeze() => _frozenTime = null;
    }
}