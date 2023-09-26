using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.DTOs.VoucherChannel
{
    public class VoucherChannelParam
    {
        public Guid PromotionId { get; set; }
        public string ChannelCode { get; set; }
        public string StoreCode { get; set; }
        public string BrandCode { get; set; }
        public int Quantity { get; set; } = 1;
    }
}
