using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.DTOs
{
    public class StoreOfPromotion
    {
        public Guid StoreId { get; set; }
        public string StoreCode { get; set; }
        public string StoreName { get; set; }
        public decimal? Group { get; set; }
        public bool IsCheck { get; set; } = false;
    }

    public class GroupStoreOfPromotion
    {
        public List<StoreOfPromotion> Stores { get; set; }
        public decimal? Group { get; set; }
    }

    public class UpdateStoreOfPromotion
    {
        public List<Guid> ListStoreId { get; set; }
        public Guid PromotionId { get; set; }
        public Guid BrandId { get; set; }
    }
}
