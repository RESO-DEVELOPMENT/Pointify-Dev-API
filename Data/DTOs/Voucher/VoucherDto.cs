using Infrastructure.Models;
using System;

namespace Infrastructure.DTOs
{
    public class VoucherDto : BaseDto
    {
        public Guid VoucherId { get; set; }
        public string VoucherCode { get; set; }
        public Guid? VoucherChannelId { get; set; }
        public Guid? VoucherGroupId { get; set; }
        public Guid? MembershipId { get; set; }
        public bool IsUsed { get; set; }
        public bool IsRedemped { get; set; }
        public DateTime? UsedDate { get; set; }
        public DateTime? RedempedDate { get; set; }
        public bool IsActive { get; set; }
        public int Index { get; set; }
        public Guid? PromotionId { get; set; }
        public Guid? PromotionTierId { get; set; }
        public String OrderId { get; set; }
        public Guid TransactionId { get; set; }
        public virtual VoucherGroup VoucherGroup { get; set; }
    }

    public class PromotionVoucherCount
    {
        public int Total { get; set; } = 0;
        public int Unused { get; set; } = 0;
        public int Redemped { get; set; } = 0;
        public int Used { get; set; } = 0;
    }

    public class CheckVoucherDto
    {
        public Models.Voucher VoucherInfo { get; set; } = new Models.Voucher();
        public dynamic Order { get; set; }
    }
}
