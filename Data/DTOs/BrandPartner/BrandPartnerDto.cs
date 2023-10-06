using System;

namespace Infrastructure.DTOs.BrandPartner
{
    public class BrandPartnerDto
    {
        public Guid BrandId { get; set; }
        public Guid? BrandPartnerId { get; set; }
    }
    public class UpdateBalance
    {
        public Guid BrandPartnerId { get; set; }
        public decimal? PartnerBalance { get; set; }
    }
}
