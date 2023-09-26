using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.DTOs
{
    public class ChannelOfPromotion
    {
        public Guid ChannelId { get; set; }
        public string ChannelName { get; set; }
        public string ChannelCode { get; set; }
        public decimal? Group { get; set; }
        public bool IsCheck { get; set; } = false;
    }

    public class GroupChannelOfPromotion
    {
        public List<ChannelOfPromotion> Channels { get; set; }
        public decimal? Group { get; set; }
    }

    public class UpdateChannelOfPromotion
    {
        public List<Guid> ListChannelId { get; set; }
        public Guid PromotionId { get; set; }
        public Guid BrandId { get; set; }
    }
}
