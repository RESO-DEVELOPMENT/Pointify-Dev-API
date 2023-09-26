using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.DTOs
{
    public class ConditionParamDto
    {
        public ConditionRuleDto conditionRule { get; set; }
        public List<ProductConditionDto> productConditions { get; set; }
        public List<OrderConditionDto> orderConditions { get; set; }
        public List<MembershipConditionDto> membershipConditions { get; set; }

        public ConditionParamDto()
        {
            productConditions = new List<ProductConditionDto>();
            orderConditions = new List<OrderConditionDto>();
            membershipConditions = new List<MembershipConditionDto>();
        }
    }
}
