using Infrastructure.Helper;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;

namespace ApplicationCore.Services
{
    public interface IHubNotify
    {
        public Task ShowMessage(NotifyMessage item);
        public Task SendSuccessMessage(string message);
        public Task SendInfoMessage(string message);
        public Task SendErrorMessage(string message);
    }
    public class HubNotify : IHubNotify
    {
        private readonly HubConnection connection;
        private const string URL = AppConstant.URL + "notify/message";

        public HubNotify()
        {
            connection = new HubConnectionBuilder().WithUrl(URL).Build();
            connection.StartAsync();
        }

        public async Task ShowMessage(NotifyMessage item)
        {
            await connection.InvokeAsync("ShowMessage", item);
        }

        public async Task SendSuccessMessage(string message)
        {
            var messageObj = new NotifyMessage()
            {
                Icon = AppConstant.NOTIFY_MESSAGE.ICON.SUCCESS,
                Type = AppConstant.NOTIFY_MESSAGE.TYPE.SUCCESS,
                Title = AppConstant.NOTIFY_MESSAGE.TITLE.SUCCESS,
                Message = message,
            };
            await ShowMessage(item: messageObj);
        }
        public async Task SendInfoMessage(string message)
        {
            var messageObj = new NotifyMessage()
            {
                Icon = AppConstant.NOTIFY_MESSAGE.ICON.LOADING,
                Type = AppConstant.NOTIFY_MESSAGE.TYPE.INFO,
                Title = AppConstant.NOTIFY_MESSAGE.TITLE.SUCCESS,
                Message = message,
            };
            await ShowMessage(item: messageObj);
        }

        public async Task SendErrorMessage(string message)
        {
            var messageObj = new NotifyMessage()
            {
                Icon = AppConstant.NOTIFY_MESSAGE.ICON.WARNING,
                Type = AppConstant.NOTIFY_MESSAGE.TYPE.WARNING,
                Title = AppConstant.NOTIFY_MESSAGE.TITLE.WARNING,
                Message = message,
            };
            await ShowMessage(item: messageObj);
        }

    }

    public class NotifyMessage
    {
        public string Icon { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }


    }
}
