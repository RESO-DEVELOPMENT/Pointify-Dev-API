using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.DTOs
{
    public class MemberLevelDto : BaseDto
    {
        public Guid MemberLevelId { get; set; }
        public Guid BrandId { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
    }
}
