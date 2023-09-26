using System;

namespace Infrastructure.DTOs
{
    public class CustomerDto : BaseDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? FinishDate { get; set; }
        public decimal? Balance { get; set; }
        public string Type { get; set; }
        public bool? Active { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public Guid BrandId { get; set; }
        public int? Gender { get; set; }
        public DateTime? Birthday { get; set; }
        public string Phone { get; set; }
        public int? Point { get; set; }

        public virtual BrandDto Brand { get; set; }
        
    }
}
