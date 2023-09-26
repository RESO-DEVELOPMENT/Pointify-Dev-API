using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.DTOs
{
    public class PromotionStatusDto
    {
        public int Total { get; set; } = 0;
        public int Draft { get; set; } = 0;
        public int Publish { get; set; } = 0;
        public int Unpublish { get; set; } = 0;
        public int Expired { get; set; } = 0;
    }

    public class DistributionStat
    {
        public List<GroupChannel> ChannelStat { get; set; }
        public List<GroupStore> StoreStat { get; set; }
    }

    public class GroupChannel
    {
        public int GroupNo { get; set; }
        public List<ChannelVoucherStat> Channels { get; set; }
    }

    public class GroupStore
    {
        public int GroupNo { get; set; }
        public List<StoreVoucherStat> Stores { get; set; }
    }

    public class ChannelVoucherStat
    {
        public Guid ChannelId { get; set; }
        public string ChannelName { get; set; }
        public int RedempVoucherCount { get; set; } = 0;
        public int GroupNo { get; set; }
    }

    public class StoreVoucherStat
    {
        public Guid StoreId { get; set; }
        public string StoreName { get; set; }
        public int RedempVoucherCount { get; set; } = 0;
        public int GroupNo { get; set; }
    }
}
