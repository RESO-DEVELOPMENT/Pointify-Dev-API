using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.DTOs
{
    public class ConditionGroupDto : BaseDto
    {
        public Guid ConditionGroupId { get; set; } = Guid.NewGuid();
        public int GroupNo { get; set; }
        public int NextOperator { get; set; }
        public string Summary { get; set; } = "";
        public ICollection<OrderConditionDtoModel> OrderCondition { get; set; }
        public ICollection<ProductConditionModelA> ProductCondition { get; set; }
    }
}
