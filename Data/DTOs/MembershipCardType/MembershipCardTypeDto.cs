using System;

namespace Infrastructure.DTOs
{
    public class MembershipCardTypeDto
    {
        public string Name { get; set; }
        public Guid? MemberShipProgramId { get; set; }
        public bool Active { get; set; }
        public string CardImg { get; set; }
    }
}
