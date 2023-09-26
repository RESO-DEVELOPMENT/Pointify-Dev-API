using System;

namespace Infrastructure.DTOs.MemberActionType
{
    public class MemberActionTypeDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool? DelFlag { get; set; }
    }
}
