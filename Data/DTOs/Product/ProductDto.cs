using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.DTOs
{
    public class ProductDto : BaseDto
    {
        public Guid BrandId { get; set; }
        public Guid ProductId { get; set; }
        public Guid ProductCateId { get; set; }
        [StringLength(30)]
        public string CateId { get; set; }
        [StringLength(50)]
        public string Code { get; set; }
        [StringLength(80)]
        public string Name { get; set; }
    }
}
