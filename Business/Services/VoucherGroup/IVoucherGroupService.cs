
using Infrastructure.DTOs;
using Infrastructure.Helper;
using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationCore.Services
{
    public interface IVoucherGroupService : IBaseService<VoucherGroup, VoucherGroupDto>
    {
        public List<VoucherDto> GenerateBulkCodeVoucher(VoucherGroupDto dto);

        // Ẩn voucher group
        public Task<bool> HideVoucherGroup(Guid id);
        // Xóa hoàn toàn voucher group
        public Task<bool> DeleteVoucherGroup(Guid id);
        // Reject voucher group ra khỏi promotion
        public Task<bool> RejectVoucherGroup(Guid voucherGroupId, Guid promotionId);
        // Assign voucher group cho promotion
        public Task<bool> AssignVoucherGroup(Guid promotionId, Guid voucherGroupId);
        public Task<List<VoucherDto>> CreateVoucherBulk(List<VoucherDto> vouchers);
        public Task<List<Voucher>> MapVoucher(List<VoucherDto> vouchers);
        public Task<String> GetPromotionCode(Guid promotionId);

        public Task UpdateRedempedQuantity(VoucherGroup voucherGroup, int RedempedQuantity);

        public Task UpdateVoucherGroupForApplied(VoucherGroup voucherGroup);

        public Task<bool> AddMoreVoucher(Guid voucherGroupId, int quantity);

        public Task<GenericRespones<AvailableVoucherDto>> GetAvailable(int PageSize, int PageIndex, Guid BrandId);
        public Task<List<VoucherGroupForPromo>> GetVoucherGroupForPromo(Guid brandId);
        public Task<VoucherIndexInfo> CheckAvailableIndex(Guid voucherGroupId);

        public Task<VoucherGroupDetailDto> GetDetail(Guid id);
        public Task<CheckAddMoreDto> GetAddMoreInfo(Guid id);

        public Task<VoucherGroupDto> UpdateVoucherGroup(VoucherGroupDto dto);
        public Task<bool> GenerateVoucher(Guid voucherGroupId, int quantity);
    }
}
