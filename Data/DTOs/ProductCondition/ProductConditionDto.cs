using Infrastructure.Models;
using System;
using System.Collections.Generic;

namespace Infrastructure.DTOs
{
    public class ProductConditionDto : BaseDto
    {
        public Guid ProductConditionId { get; set; } = Guid.NewGuid();
        public Guid ConditionGroupId { get; set; }
        public int IndexGroup { get; set; }
        public int ProductConditionType { get; set; }
        public int ProductQuantity { get; set; }
        public string QuantityOperator { get; set; }
        public int NextOperator { get; set; }
        public List<ProductConditionMapping> ProductConditionMapping { get; set; }
    }

    public class ProductConditionModelA
    {
        public int IndexGroup { get; set; }
        public int ProductConditionType { get; set; }
        public int ProductQuantity { get; set; }
        public string QuantityOperator { get; set; }
        public int NextOperator { get; set; }
    }
}
