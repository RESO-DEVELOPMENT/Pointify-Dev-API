using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.DTOs
{
    public class TransactionDTO : BaseDto
    {
        public Guid Id { get; set; }
        public Guid BrandId { get; set; }
        public string TransactionJson { get; set; }
        public Guid VoucherId { get; set; }

        public Guid PromotionId { get; set; }

        public virtual Brand Brand { get; set; }
    }

    public class PromoTrans
    {
        public Transaction Transaction { get; set; }
        public dynamic Order { get; set; }
    }
}
