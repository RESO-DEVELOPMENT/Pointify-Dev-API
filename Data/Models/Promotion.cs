using System;
using System.Collections.Generic;

namespace Infrastructure.Models
{
    public partial class Promotion
    {
        public Promotion()
        {
            GameCampaign = new HashSet<GameCampaign>();
            MemberLevelMapping = new HashSet<MemberLevelMapping>();
            PromotionChannelMapping = new HashSet<PromotionChannelMapping>();
            PromotionStoreMapping = new HashSet<PromotionStoreMapping>();
            PromotionTier = new HashSet<PromotionTier>();
            Voucher = new HashSet<Voucher>();
        }

        public Guid PromotionId { get; set; }
        public Guid BrandId { get; set; }
        public string PromotionCode { get; set; }
        public string PromotionName { get; set; }
        public int ActionType { get; set; }
        public int PostActionType { get; set; }
        public string ImgUrl { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Exclusive { get; set; }
        public int ApplyBy { get; set; }
        public int SaleMode { get; set; }
        public int Gender { get; set; }
        public int PaymentMethod { get; set; }
        public int ForHoliday { get; set; }
        public int ForMembership { get; set; }
        public int DayFilter { get; set; }
        public int HourFilter { get; set; }
        public int Status { get; set; }
        public bool DelFlg { get; set; }
        public DateTime InsDate { get; set; }
        public DateTime UpdDate { get; set; }
        public bool HasVoucher { get; set; }
        public bool IsAuto { get; set; }
        public int? PromotionType { get; set; }

        public virtual Brand Brand { get; set; }
        public virtual ICollection<GameCampaign> GameCampaign { get; set; }
        public virtual ICollection<MemberLevelMapping> MemberLevelMapping { get; set; }
        public virtual ICollection<PromotionChannelMapping> PromotionChannelMapping { get; set; }
        public virtual ICollection<PromotionStoreMapping> PromotionStoreMapping { get; set; }
        public virtual ICollection<PromotionTier> PromotionTier { get; set; }
        public virtual ICollection<Voucher> Voucher { get; set; }
    }
}
