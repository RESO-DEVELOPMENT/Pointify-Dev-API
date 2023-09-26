using System;
using System.Collections.Generic;

namespace Infrastructure.Models
{
    public partial class GameMaster
    {
        public GameMaster()
        {
            GameCampaign = new HashSet<GameCampaign>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public int MinItem { get; set; }
        public int MaxItem { get; set; }
        public bool DelFlg { get; set; }
        public DateTime InsDate { get; set; }
        public DateTime UpdDate { get; set; }

        public virtual ICollection<GameCampaign> GameCampaign { get; set; }
    }
}
