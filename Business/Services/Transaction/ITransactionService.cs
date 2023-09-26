using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Request;
using Infrastructure.DTOs;
using Infrastructure.Helper;
using Infrastructure.Models;

namespace ApplicationCore.Services
{
    public interface ITransactionService : IBaseService<Transaction, TransactionDTO>
    {
        public Task<Order> PlaceOrder(Guid brandId, Order order, Guid deviceId);
        public Task<Order> PlaceOrderForChannel(Order order, string channelCode);
        public Task<GenericRespones<PromoTrans>> GetPromoTrans(Guid promotionId, PagingRequestParam param);
    }
}
