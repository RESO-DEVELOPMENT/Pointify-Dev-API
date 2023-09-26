using Infrastructure.DTOs;
using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.DTOs
{
    public class DeleteTierRequestParam
    {
        public Guid PromotionId { get; set; }
        public Guid PromotionTierId { get; set; }
        //public Guid ActionId { get; set; }
        //public Guid GiftId { get; set; }
    }
}
