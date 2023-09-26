using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.DTOs
{
    public class StoreGameCampaignMappingDto
    {
        public Guid StoreGameCampaignId { get; set; }
        public Guid GameCampaignId { get; set; }
        public Guid StoreId { get; set; }
    }
}
