using System;
using System.Collections.Generic;

namespace Infrastructure.Models
{
    public partial class Store
    {
        public Store()
        {
            Device = new HashSet<Device>();
            PromotionStoreMapping = new HashSet<PromotionStoreMapping>();
            StoreGameCampaignMapping = new HashSet<StoreGameCampaignMapping>();
            Voucher = new HashSet<Voucher>();
        }

        public Guid StoreId { get; set; }
        public Guid BrandId { get; set; }
        public string StoreCode { get; set; }
        public string StoreName { get; set; }
        public bool DelFlg { get; set; }
        public DateTime? InsDate { get; set; }
        public DateTime? UpdDate { get; set; }
        public int Group { get; set; }

        public virtual Brand Brand { get; set; }
        public virtual ICollection<Device> Device { get; set; }
        public virtual ICollection<PromotionStoreMapping> PromotionStoreMapping { get; set; }
        public virtual ICollection<StoreGameCampaignMapping> StoreGameCampaignMapping { get; set; }
        public virtual ICollection<Voucher> Voucher { get; set; }
    }
}
