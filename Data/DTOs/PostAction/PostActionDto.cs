using Infrastructure.Models;
using System;
using System.Collections.Generic;

namespace Infrastructure.DTOs
{
    public class GiftDto : BaseDto
    {
        public Guid GiftId { get; set; }
        public Guid? PromotionTierId { get; set; }
        public int PostActionType { get; set; }
        public decimal? BonusPoint { get; set; }
        public Guid? GiftVoucherGroupId { get; set; }
        public Guid? GameCampaignId { get; set; }
        public string Name { get; set; }
        public Guid BrandId { get; set; }
        public List<GiftProductMapp> ListProduct { get; set; }
        public List<GiftProductMapping> ListProductMapp { get; set; }

    }

    public class GiftProductMapp
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
