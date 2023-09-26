using System;

namespace Infrastructure.DTOs.MembershipProgram
{
    public class MembershipProgramDto
    {
        public Guid Id { get; set; }
        public Guid BrandId { get; set; }
        public string NameOfProgram { get; set; }
        public DateTime? StartDay { get; set; } = DateTime.Now;
        public DateTime? EndDay { get; set; } = DateTime.Now;
        public bool? DelFlg { get; set; }
        public string TermAndConditions { get; set; }
        public string Status { get; set; }
    }
}
