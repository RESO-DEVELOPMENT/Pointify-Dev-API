using Infrastructure.Models;
using System.Collections.Generic;

namespace ApplicationCore.Models
{
    public class ProductConditionModel : ConditionModel
    {
        public string ProductConditionType { get; set; }
        public List<Product> Products { get; set; }
        public decimal ProductQuantity { get; set; }
        public string QuantityOperator { get; set; }
    }
}
