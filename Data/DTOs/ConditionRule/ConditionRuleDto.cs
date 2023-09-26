using System;
using System.Collections.Generic;

namespace Infrastructure.DTOs
{
    public class ConditionRuleDto : BaseDto
    {
        public Guid ConditionRuleId { get; set; } = Guid.NewGuid();
        public Guid? BrandId { get; set; }
        public string RuleName { get; set; }
        public string Description { get; set; }
        public virtual ICollection<ConditionGroupDto> ConditionGroup { get; set; }
    }

    public class UpdateConditionDto
    {
        public ConditionRequestParam ConditionRule { get; set; }
        public List<ConditionGroupDto> ConditionGroups { get; set; }
    }

    public class ConditionRuleDtoModel
    {
        public Guid? BrandId { get; set; }
        public string RuleName { get; set; }
        public string Description { get; set; }
        public virtual ICollection<ConditionGroupDto> ConditionGroup { get; set; }

    }
}
