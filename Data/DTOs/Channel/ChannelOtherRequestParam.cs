using System;
using System.Collections.Generic;

namespace Infrastructure.DTOs
{
    public class ChannelOtherPlaceOrder
    {

    }

    public class OtherEffect
    {
        public Guid PromotionId { get; set; }
        public Guid? PromotionTierId { get; set; }
        public int TierIndex { get; set; }
        public string PromotionName { get; set; }
        public string ConditionRuleName { get; set; }
        public string ImgUrl { get; set; }
        public string Description { get; set; }
        public string EffectType { get; set; }
        public Object Prop { get; set; }
    }
    public class ChannelOtherRequestParam
    {
        public string ApiKey { get; set; }
        public string ChannelCode { get; set; }
        public string BrandCode { get; set; }
        public ChannelOtherRequestParam()
        {
            CartItems = new List<ChannelItem>();
            Customer = new ChannelCustomer();
        }
        public string Id { get; set; }
        public DateTime BookingDate { get; set; }
        public List<ChannelItem> CartItems { get; set; }
        public List<ChannelCouponCode> Vouchers { get; set; }
        public decimal Amount { get; set; }
        public decimal ShippingFee { get; set; }
        public ChannelCustomer Customer { get; set; }
        public string Hash { get; set; }

    }
    public class ChannelCustomer
    {
        public string CustomerName { get; set; }
        public string CustomerPhoneNo { get; set; }
        public int CustomerGender { get; set; } = 3;
        public string CustomerLevel { get; set; }

    }
    public class ChannelCouponCode
    {
        public string PromotionCode { get; set; }
        public string VoucherCode { get; set; }
    }
    public class ChannelItem
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Discount { get; set; }
        public decimal DiscountFromOrder { get; set; }
        public decimal Total { get; set; }
        public string UrlImg { get; set; }
    }
}
