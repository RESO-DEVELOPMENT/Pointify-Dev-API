using System;

namespace Infrastructure.DTOs
{
    public class OrderConditionDto : BaseDto
    {
        public Guid OrderConditionId { get; set; } = Guid.NewGuid();
        public Guid ConditionGroupId { get; set; }
        public int NextOperator { get; set; }
        public int IndexGroup { get; set; }
        public int Quantity { get; set; }
        public string QuantityOperator { get; set; }
        public decimal Amount { get; set; }
        public string AmountOperator { get; set; }
    }

    public class OrderConditionDtoModel
    {
        public int NextOperator { get; set; }
        public int IndexGroup { get; set; }
        public int Quantity { get; set; }
        public string QuantityOperator { get; set; }
        public decimal Amount { get; set; }
        public string AmountOperator { get; set; }
    }
}
