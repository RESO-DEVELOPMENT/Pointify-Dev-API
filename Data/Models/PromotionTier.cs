using System;
using System.Collections.Generic;

namespace Infrastructure.Models
{
    public partial class PromotionTier
    {
        public Guid PromotionTierId { get; set; }
        public Guid? ConditionRuleId { get; set; }
        public Guid? ActionId { get; set; }
        public Guid? PromotionId { get; set; }
        public Guid? GiftId { get; set; }
        public DateTime InsDate { get; set; }
        public DateTime UpdDate { get; set; }
        public string Summary { get; set; }
        public int TierIndex { get; set; }
        public Guid? VoucherGroupId { get; set; }
        public int? Priority { get; set; }
        public int? VoucherQuantity { get; set; }
        public string ProductCode { get; set; }

        public virtual Action Action { get; set; }
        public virtual ConditionRule ConditionRule { get; set; }
        public virtual Gift Gift { get; set; }
        public virtual Promotion Promotion { get; set; }
        public virtual VoucherGroup VoucherGroup { get; set; }
    }
}
