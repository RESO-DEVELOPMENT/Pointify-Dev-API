using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.DTOs.Condition
{
    public class ConditionGroupModel
    {
        public decimal GroupNo { get; set; }
        public int NextOperator { get; set; }
        public bool IsMatch { get; set; } = true;

        public ConditionGroupModel(decimal groupNo, int nextOperator, bool isMatch)
        {
            GroupNo = groupNo;
            NextOperator = nextOperator;
            IsMatch = isMatch;
        }
    }
}
