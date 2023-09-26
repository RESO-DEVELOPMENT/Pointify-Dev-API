using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.DTOs
{
    public class VoucherChannelDto : BaseDto
    {
        public Guid VoucherChannelId { get; set; }
        public Guid? VoucherGroupId { get; set; }
        public Guid? PromotionId { get; set; }
        public Guid? ChannelId { get; set; }
    }
}
