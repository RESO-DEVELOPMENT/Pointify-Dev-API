using System;

namespace Infrastructure.DTOs
{
    public class BrandDto : BaseDto
    {
        public Guid BrandId { get; set; }
        public string BrandCode { get; set; }
        public string PhoneNumber { get; set; }
        public string ImgUrl { get; set; }
        public string BrandName { get; set; }
        public string CompanyName { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string BrandEmail { get; set; }
    }
}
