using System;

namespace Infrastructure.DTOs
{
    public class MembershipDto : BaseDto
    {
        public Guid MembershipId { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Fullname { get; set; }

        public int? Gender { get; set; }
        public Guid? MemberProgramId { get; set; }
        public Guid? MemberLevelId { get; set; }
    }

    public class UpMembership
    {
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Fullname { get; set; }
    }
}