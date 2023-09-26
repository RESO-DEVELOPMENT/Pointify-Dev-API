using System;
using System.Collections.Generic;

namespace Infrastructure.Models
{
    public partial class ConditionRule
    {
        public ConditionRule()
        {
            ConditionGroup = new HashSet<ConditionGroup>();
            PromotionTier = new HashSet<PromotionTier>();
        }

        public Guid ConditionRuleId { get; set; }
        public Guid BrandId { get; set; }
        public string RuleName { get; set; }
        public string Description { get; set; }
        public bool DelFlg { get; set; }
        public DateTime InsDate { get; set; }
        public DateTime UpdDate { get; set; }

        public virtual Brand Brand { get; set; }
        public virtual ICollection<ConditionGroup> ConditionGroup { get; set; }
        public virtual ICollection<PromotionTier> PromotionTier { get; set; }
    }
}
