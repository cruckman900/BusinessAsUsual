using Microsoft.AspNetCore.SignalR;

namespace BusinessAsUsual.Admin.Hubs
{
    public class SmartCommitHub : Hub
    {
        public async Task BroadcastCommit(string message)
        {
            await Clients.All.SendAsync("ReceiveCommit", message);
        }
    }
}