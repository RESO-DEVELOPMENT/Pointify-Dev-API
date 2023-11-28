using Infrastructure.Models;
using System.Collections.Generic;
using System;
using Infrastructure.DTOs.MemberLevel;

namespace Infrastructure.DTOs.Membership
{
    public class MembershipResponse
    {
        public Guid MembershipId { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Fullname { get; set; }
        public bool DelFlg { get; set; }
        public int? Gender { get; set; }
        public virtual MemberLevelResponse MemberLevel { get; set; }
    }
}
