using System;
using System.Collections;
using System.Collections.Generic;

namespace Infrastructure.DTOs.ScanMembership
{
    public class ScanMembershipResponse
    {
        public Guid MembershipId { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public int? Gender { get; set; }
        public string MemberLevelName { get; set; }
        public decimal Point { get; set; }
        public decimal Balance { get; set; }
    }
}
