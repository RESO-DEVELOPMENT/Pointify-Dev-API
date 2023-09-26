using System;
using System.Collections.Generic;

namespace Infrastructure.Models
{
    public partial class Voucher
    {
        public Guid VoucherId { get; set; }
        public string VoucherCode { get; set; }
        public Guid? ChannelId { get; set; }
        public Guid? StoreId { get; set; }
        public Guid VoucherGroupId { get; set; }
        public Guid? MembershipId { get; set; }
        public bool IsUsed { get; set; }
        public bool IsRedemped { get; set; }
        public DateTime? UsedDate { get; set; }
        public DateTime? RedempedDate { get; set; }
        public DateTime InsDate { get; set; }
        public DateTime UpdDate { get; set; }
        public Guid? PromotionId { get; set; }
        public int? Index { get; set; }
        public Guid? GameCampaignId { get; set; }
        public Guid? PromotionTierId { get; set; }
        public string OrderId { get; set; }
        public Guid? TransactionId { get; set; }

        public virtual Channel Channel { get; set; }
        public virtual GameCampaign GameCampaign { get; set; }
        public virtual Membership Membership { get; set; }
        public virtual Promotion Promotion { get; set; }
        public virtual Store Store { get; set; }
        public virtual VoucherGroup VoucherGroup { get; set; }
    }
}
