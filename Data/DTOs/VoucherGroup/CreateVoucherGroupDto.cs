using Infrastructure.Models;
using System;
using System.Collections.Generic;

namespace Infrastructure.DTOs
{
    public class CreateVoucherGroupDto : BaseDto
    {
        public Guid VoucherGroupId { get; set; }
        public Guid? PromotionId { get; set; }
        public Guid? BrandId { get; set; }
        public string VoucherName { get; set; }
        public int Quantity { get; set; } = 0;
        public int UsedQuantity { get; set; } = 0;
        public int RedempedQuantity { get; set; } = 0;
        public string Charset { get; set; }
        public string Postfix { get; set; }
        public string Prefix { get; set; }
        public string CustomCharset { get; set; }
        public Guid? ConditionRuleId { get; set; }
        public Guid? ActionId { get; set; }
        public Guid? GiftId { get; set; }
        public int CodeLength { get; set; }
        public string ImgUrl { get; set; }

        public virtual ICollection<VoucherChannelDto> VoucherChannel { get; set; }
    }
}
