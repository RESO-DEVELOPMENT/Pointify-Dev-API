using Infrastructure.Models;
using System;

namespace Infrastructure.DTOs
{
    public class MemberDto : BaseDto
    {
        public Guid Id { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public Guid? MemberProgramId { get; set; }
        public Guid? CustomerId { get; set; }

    }
}
