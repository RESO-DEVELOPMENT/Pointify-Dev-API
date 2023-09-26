using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.DTOs
{
    public class PromotionStoreMappingDto : BaseDto
    {
        public Guid Id { get; set; }
        public Guid? StoreId { get; set; }
        public Guid? PromotionId { get; set; }
    }
}
