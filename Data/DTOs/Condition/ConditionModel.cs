
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationCore.Models
{
    public class ConditionModel
    {
        public Guid Id { get; set; }
        public int Index { get; set; }
        public int NextOperator { get; set; }

        public bool IsMatch { get; set; } = true;

        public override string ToString()
        {
            string opera = NextOperator.Equals("1") ? "OR" : "AND";
            return IsMatch + " " + opera + " ";
        }
    }
}
