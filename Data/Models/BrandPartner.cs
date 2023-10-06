using System;
using System.Collections.Generic;

namespace Infrastructure.Models
{
    public partial class BrandPartner
    {
        public Guid Id { get; set; }
        public Guid BrandId { get; set; }
        public Guid? BrandPartnerId { get; set; }
        public decimal? PartnerBalance { get; set; }
        public bool DeFlag { get; set; }
        public DateTime InsDate { get; set; }
        public DateTime UpdDate { get; set; }

        public virtual Brand Brand { get; set; }
        public virtual Brand BrandPartnerNavigation { get; set; }
    }
}
