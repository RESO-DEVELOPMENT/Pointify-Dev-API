using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromotionEngineAPI.Hubs
{
    public class VoucherHub : Hub
    {
        public async Task GeneratingVoucher(object item)
        {
            await Clients.All.SendAsync("GeneratingVouchers", item);
        }
        public async Task GeneratedVoucher(object item)
        {
            await Clients.All.SendAsync("GeneratedVouchers", item);
        }
        public async Task ProcessingNoti(object item)
        {
            await Clients.All.SendAsync("InsertingVouchers", item);
        }
        public async Task DoneNoti(object item)
        {
            await Clients.All.SendAsync("InsertedVouchers", item);
        }
        public async Task ErrorNoti(object item)
        {
            await Clients.All.SendAsync("Error", item);
        }
    }
}
