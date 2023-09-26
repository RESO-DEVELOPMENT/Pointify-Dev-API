using Infrastructure.Models;
using System.Collections.Generic;
using System;

namespace Infrastructure.DTOs
{
    public class MemberShipCardDto
    {
        public Guid Id { get; set; }
        public Guid MemberId { get; set; }
        public string MembershipCardCode { get; set; }
        public bool? Active { get; set; }
        public DateTime? CreatedTime { get; set; }
        public Guid BrandId { get; set; }
        public Guid MembershipCardTypeId { get; set; }
        public string PhysicalCardCode { get; set; }

    }
}
