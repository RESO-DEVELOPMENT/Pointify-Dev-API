using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.DTOs
{
    public class ConditionRequestParam
    {
        public Guid ConditionRuleId { get; set; }
        public Guid? BrandId { get; set; }
        [StringLength(50)]
        public string RuleName { get; set; }
        [StringLength(200)]
        public string Description { get; set; }
        public DateTime InsDate { get; set; }
        public DateTime UpdDate { get; set; }
    }
}
