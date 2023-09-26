using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace WebAPI.Hubs
{
    public class NotifyMessageHub : Hub
    {
        public async Task ShowMessage(object item)
        {
            await Clients.All.SendAsync("ShowMessage", item);
        }
    }
}
