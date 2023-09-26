using System;
using System.Collections.Generic;

namespace Infrastructure.DTOs
{
    public class ConditionGroupResponse
    {
        public Guid ConditionGroupId { get; set; }
        public Guid ConditionRuleId { get; set; }
        public int GroupNo { get; set; }
        public int NextOperator { get; set; }
        public List<Object> Conditions { get; set; }
        public string Summary { get; set; }

    }

    public class ConditionRuleResponse
    {
        public Guid ConditionRuleId { get; set; }
        public Guid? BrandId { get; set; }
        public string RuleName { get; set; }
        public string Description { get; set; }
        public List<ConditionGroupResponse> ConditionGroups { get; set; }
        public Guid? PromotionId { get; set; }
        public string? PromotionName { get; set; }
        public DateTime InsDate { get; set; }
        public DateTime UpdDate { get; set; }

    }
}
