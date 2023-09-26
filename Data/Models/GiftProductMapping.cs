using System;
using System.Collections.Generic;

namespace Infrastructure.Models
{
    public partial class GiftProductMapping
    {
        public Guid Id { get; set; }
        public Guid GiftId { get; set; }
        public Guid ProductId { get; set; }
        public DateTime InsDate { get; set; }
        public DateTime UpdDate { get; set; }
        public int? Quantity { get; set; }

        public virtual Gift Gift { get; set; }
        public virtual Product Product { get; set; }
    }
}
