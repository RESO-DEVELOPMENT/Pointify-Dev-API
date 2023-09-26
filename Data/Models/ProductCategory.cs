using System;
using System.Collections.Generic;

namespace Infrastructure.Models
{
    public partial class ProductCategory
    {
        public ProductCategory()
        {
            Product = new HashSet<Product>();
        }

        public Guid ProductCateId { get; set; }
        public Guid BrandId { get; set; }
        public string CateId { get; set; }
        public string Name { get; set; }
        public bool DelFlg { get; set; }
        public DateTime UpdDate { get; set; }
        public DateTime InsDate { get; set; }

        public virtual Brand Brand { get; set; }
        public virtual ICollection<Product> Product { get; set; }
    }
}
