using System;
using System.Collections.Generic;

namespace Infrastructure.Models
{
    public partial class VoucherGroup
    {
        public VoucherGroup()
        {
            PromotionTier = new HashSet<PromotionTier>();
            Voucher = new HashSet<Voucher>();
        }

        public Guid VoucherGroupId { get; set; }
        public Guid BrandId { get; set; }
        public string VoucherName { get; set; }
        public int Quantity { get; set; }
        public int UsedQuantity { get; set; }
        public int RedempedQuantity { get; set; }
        public bool DelFlg { get; set; }
        public DateTime InsDate { get; set; }
        public DateTime UpdDate { get; set; }
        public string Charset { get; set; }
        public string Postfix { get; set; }
        public string Prefix { get; set; }
        public string CustomCharset { get; set; }
        public Guid? ActionId { get; set; }
        public Guid? GiftId { get; set; }
        public int? CodeLength { get; set; }
        public string ImgUrl { get; set; }

        public virtual Action Action { get; set; }
        public virtual Brand Brand { get; set; }
        public virtual Gift Gift { get; set; }
        public virtual ICollection<PromotionTier> PromotionTier { get; set; }
        public virtual ICollection<Voucher> Voucher { get; set; }
    }
}
