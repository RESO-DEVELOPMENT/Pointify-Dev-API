using System;
using System.Collections.Generic;

namespace Infrastructure.Models
{
    public partial class ConditionGroup
    {
        public ConditionGroup()
        {
            OrderCondition = new HashSet<OrderCondition>();
            ProductCondition = new HashSet<ProductCondition>();
        }

        public Guid ConditionGroupId { get; set; }
        public Guid ConditionRuleId { get; set; }
        public int GroupNo { get; set; }
        public int NextOperator { get; set; }
        public DateTime InsDate { get; set; }
        public DateTime UpdDate { get; set; }
        public string Summary { get; set; }

        public virtual ConditionRule ConditionRule { get; set; }
        public virtual ICollection<OrderCondition> OrderCondition { get; set; }
        public virtual ICollection<ProductCondition> ProductCondition { get; set; }
    }
}
