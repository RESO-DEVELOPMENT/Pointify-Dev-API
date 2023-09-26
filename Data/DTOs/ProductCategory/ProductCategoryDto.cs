using System;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.DTOs
{
    public class ProductCategoryDto : BaseDto
    {
        public Guid ProductCateId { get; set; }
        public Guid BrandId { get; set; }
        [StringLength(30)]
        public string CateId { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
    }
}
