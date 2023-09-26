using ApplicationCore.Request;
using Infrastructure.DTOs;

using Infrastructure.DTOs.VoucherChannel;
using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApplicationCore.Services
{
    public interface IVoucherService : IBaseService<Voucher, VoucherDto>
    {
        public Task<List<Promotion>> CheckVoucher(CustomerOrderInfo order);

        public Task<List<Voucher>> GetVouchersForChannel(PromotionChannelMapping voucherChannel, VoucherGroup voucherGroup, VoucherChannelParam channelParam);

        public Task<int> UpdateVoucherApplied(Guid transactionId, CustomerOrderInfo order, Guid storeId, Guid promotionTierId);

        public Task<VoucherParamResponse> GetVoucherForCustomer(VoucherGroupDto voucherGroupDto);

        public Task<VoucherForCustomerModel> GetVoucherForCusOnSite(VoucherForCustomerModel param, Guid promotionId, Guid tierId);

        public string Encrypt(string Encryptval);

        public string Decrypt(string DecryptText);

        public Task<PromotionVoucherCount> PromoVoucherCount(Guid promotionId, Guid voucherGroupId);

        public Task<CheckVoucherDto> GetCheckVoucherInfo(string searchCode, Guid voucherGroupId);
        public Task<int> UpdateVoucherOther(Guid transactionId, CustomerOrderInfo order, Guid promotionTierId, Channel channel, Store store);
        public Task<dynamic> GetPromotionCodeByVoucherCode(string voucherCode);
    }

}
