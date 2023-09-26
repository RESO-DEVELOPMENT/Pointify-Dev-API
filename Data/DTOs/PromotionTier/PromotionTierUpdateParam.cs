using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.DTOs
{
    public class PromotionTierUpdateParam
    {
        public Guid PromotionId { get; set; }
        public ConditionRequestParam ConditionRule { get; set; }
        public List<ConditionGroupDto> ConditionGroups { get; set; }
        public ActionUpdateParam Action { get; set; }
        public GiftUpdateParam Gift { get; set; }
        public Guid PromotionTierId { get; set; }
        public string Summary { get; set; }
    }

    public class ActionUpdateParam
    {
        public Guid ActionId { get; set; }
        [StringLength(1)]
        public string ActionType { get; set; }
        [StringLength(1)]
        public string DiscountType { get; set; }
        [Range(0, 999999)]
        public decimal? DiscountQuantity { get; set; }
        [Range(0, 9999999999)]
        public decimal? DiscountAmount { get; set; }
        [Range(0.00, 100.00)]
        public decimal? DiscountPercentage { get; set; }
        [Range(0, 9999999999)]
        public decimal? FixedPrice { get; set; }
        [Range(0, 9999999999)]
        public decimal? MaxAmount { get; set; }
        [Range(0, 9999999999)]
        public decimal? MinPriceAfter { get; set; }
        [Range(0, 9999999999)]
        public decimal? OrderLadderProduct { get; set; }
        [Range(0, 9999999999)]
        public decimal? LadderPrice { get; set; }
        [Range(0, 9999999999)]
        public decimal? BundlePrice { get; set; }
        [Range(0, 999999)]
        public decimal? BundleQuantity { get; set; }
        [StringLength(1)]
        public string BundleStrategy { get; set; }
        public List<Guid> ListProduct { get; set; }
    }

    public class GiftUpdateParam
    {
        public Guid GiftId { get; set; }
        [StringLength(1)]
        public string ActionType { get; set; }
        [StringLength(2)]
        public string DiscountType { get; set; }
        [Range(0, 999999)]
        public decimal? GiftQuantity { get; set; }
        public Guid GiftPromotionId { get; set; }
        [Range(0.00, 9999999999.00)]
        public decimal? BonusPoint { get; set; }
        public List<Guid> ListProduct { get; set; }
    }
}
