using System;

namespace Infrastructure.DTOs
{
    public class PromotionTierDto : BaseDto
    {
        public Guid PromotionTierId { get; set; }
        public Guid? ConditionRuleId { get; set; }
        public Guid? ActionId { get; set; }
        public Guid? PromotionId { get; set; }
        public Guid? GiftId { get; set; }
        public string Summary { get; set; } = "";
        public int TierIndex { get; set; } = 0;
        public Guid? VoucherGroupId { get; set; }
        public int VoucherQuantity { get; set; } = 0;
        public int Priority { get; set; }

        public int MoreQuantity { get; set; } = 0;
        public string? ProductCode { get; set; }

    }
}
