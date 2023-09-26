
using ApplicationCore.Request;
using Infrastructure.DTOs;
using Infrastructure.DTOs.VoucherChannel;
using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationCore.Services
{
    public interface IChannelService : IBaseService<Channel, ChannelDto>
    {
        public Task<VoucherForChannelResponse> GetVouchersForChannel(VoucherChannelParam channelParam);
        public Task<List<PromotionInfomation>> GetPromotionsForChannel(VoucherChannelParam channelParam, string promotionType);

        public Task<List<GroupChannelOfPromotion>> GetChannelOfPromotion(Guid promotionId, Guid brandId);
        public Task<List<GroupChannelOfPromotion>> UpdateChannelOfPromotion(UpdateChannelOfPromotion dto);
        public Task<CustomerOrderInfo> DecryptAttribute(ChannelOtherRequestParam param);
        public Task<VoucherForCustomerModel> DecryptCustomer(VoucherForOtherChannel param);
    }
}
