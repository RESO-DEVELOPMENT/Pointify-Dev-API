using System;
using System.Collections.Generic;

namespace Infrastructure.DTOs
{
    public class PromotionDto : BaseDto
    {
        public Guid PromotionId { get; set; } = Guid.NewGuid();
        public Guid BrandId { get; set; }
        public string PromotionCode { get; set; }
        public string PromotionName { get; set; }
        public int ActionType { get; set; }
        public int PostActionType { get; set; }
        public string ImgUrl { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Exclusive { get; set; } = -1;
        public int ApplyBy { get; set; }
        public int SaleMode { get; set; }
        public int Gender { get; set; }
        public int PaymentMethod { get; set; }
        public int ForHoliday { get; set; }
        public int ForMembership { get; set; }
        public int DayFilter { get; set; }
        public int HourFilter { get; set; }
        public int Status { get; set; }
        public bool? HasVoucher { get; set; }
        public bool? IsAuto { get; set; }
        public Guid? VoucherGroupId { get; set; }
        public int VoucherQuantity { get; set; } = 0;
        public Guid? ConditionRuleId { get; set; }
        public int? PromotionType { get; set; } = 0;


        public virtual ICollection<PromotionStoreMappingDto> PromotionStoreMapping { get; set; }
        public virtual ICollection<MemberLevelMappingDto> MemberLevelMapping { get; set; }
    }

    public class PromotionModel
    {
        public Guid BrandId { get; set; }
        public string PromotionCode { get; set; }
        public string PromotionName { get; set; }
        public int ActionType { get; set; }
        public int PostActionType { get; set; }
        public string ImgUrl { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public int Exclusive { get; set; } = -1;
        public int ApplyBy { get; set; }
        public int SaleMode { get; set; }
        public int Gender { get; set; }
        public int PaymentMethod { get; set; }
        public int ForHoliday { get; set; }
        public int ForMembership { get; set; }
        public int DayFilter { get; set; }
        public int HourFilter { get; set; }
        public int Status { get; set; }
    }
}
