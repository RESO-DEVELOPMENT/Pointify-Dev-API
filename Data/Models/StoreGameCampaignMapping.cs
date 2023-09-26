using System;
using System.Collections.Generic;

namespace Infrastructure.Models
{
    public partial class StoreGameCampaignMapping
    {
        public Guid StoreGameCampaignId { get; set; }
        public Guid GameCampaignId { get; set; }
        public Guid StoreId { get; set; }
        public DateTime InsDate { get; set; }
        public DateTime UpdDate { get; set; }

        public virtual GameCampaign GameCampaign { get; set; }
        public virtual Store Store { get; set; }
    }
}
