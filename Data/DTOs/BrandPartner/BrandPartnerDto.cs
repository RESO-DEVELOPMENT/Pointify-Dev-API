using System;

namespace Infrastructure.DTOs.BrandPartner
{
    public class BrandPartnerDto
    {
        public Guid BrandId { get; set; }
        public Guid? BrandPartnerId { get; set; }
        public decimal? PartnerBalance { get; set; }

        public BrandPartnerDto(Guid BrandId, Guid BrandPartnerId, decimal PartnerBalance)
        {
            this.BrandId = BrandId;
            this.BrandPartnerId = BrandPartnerId;
            this.PartnerBalance = PartnerBalance;
        }
    }
}
