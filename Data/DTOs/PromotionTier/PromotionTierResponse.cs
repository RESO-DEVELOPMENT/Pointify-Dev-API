using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.DTOs
{
    public class PromotionTierResponseParam
    {
        public Guid PromotionTierId { get; set; }
        public Guid? ConditionRuleId { get; set; }
        public Guid? ActionId { get; set; }
        public Guid? PromotionId { get; set; }
        public Guid? GiftId { get; set; }
        public string Summary { get; set; }

        public ConditionRuleResponse ConditionRule { get; set; }
        public virtual ActionTierDto Action { get; set; }
        public virtual GiftTierDto Gift { get; set; }
    }

    public class ActionTierDto
    {
        public Guid ActionId { get; set; }
        public Guid PromotionTierId { get; set; }
        public string ActionType { get; set; }
        public string DiscountType { get; set; }
        public decimal? DiscountQuantity { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal? FixedPrice { get; set; }
        public decimal? MaxAmount { get; set; }
        public decimal? MinPriceAfter { get; set; }
        public decimal? OrderLadderProduct { get; set; }
        public decimal? LadderPrice { get; set; }
        public decimal? BundlePrice { get; set; }
        public decimal? BundleQuantity { get; set; }
        public string BundleStrategy { get; set; }
        public List<ProductDto> productList { get; set; }
    }

    public class GiftTierDto
    {
        public Guid GiftId { get; set; }
        public Guid PromotionTierId { get; set; }
        public string ActionType { get; set; }
        public string DiscountType { get; set; }
        public decimal? GiftQuantity { get; set; }
        public string GiftProductCode { get; set; }
        public string GiftName { get; set; }
        public Guid? GiftPromotionId { get; set; }
        public decimal? BonusPoint { get; set; }
        public List<ProductDto> productList { get; set; }
    }
}
