using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.DTOs
{
    public class ConditionRuleUpdateParam
    {
        public Guid ConditionRuleId { get; set; }
        [StringLength(30)]
        public string RuleName { get; set; }
        [StringLength(200)]
        public string Description { get; set; }
        public PromotionTier PromotionTier { get; set; }
    }
}
