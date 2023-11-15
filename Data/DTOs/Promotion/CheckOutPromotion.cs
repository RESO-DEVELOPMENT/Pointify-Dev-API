using System;
using System.Collections.Generic;

namespace Infrastructure.DTOs.Promotion
{
    public class CheckOutPromotion
    {
        public string StoreCode { get; set; }
        public Guid? UserId { get; set; }
        public List<Effects>? listEffect { get; set; }
        public string? VoucherCode { get; set; }
        public decimal FinalAmount { get; set; }
        public decimal BonusPoint { get; set; }
        public string InvoiceId { get; set; }
    }
    public class Effects
    {
        public Guid PromotionId { get; set; }
        public string effectType { get; set; }
    }
}
