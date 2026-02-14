using Microsoft.AspNetCore.SignalR;

namespace BusinessAsUsual.Admin.Hubs
{
    /// <summary>
    /// SignalR hub for broadcasting smart commit messages to connected clients.
    /// </summary>
    public class SmartCommitHub : Hub
    {
        /// <summary>
        /// Sends a commit message to all connected clients.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task BroadcastCommit(string message)
        {
            await Clients.All.SendAsync("ReceiveCommit", message);
        }
    }
}