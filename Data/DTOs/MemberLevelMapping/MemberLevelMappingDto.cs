using System;

namespace Infrastructure.DTOs
{
    public class MemberLevelMappingDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? MemberLevelId { get; set; }
        public Guid? PromotionId { get; set; }
        public DateTime? InsDate { get; set; } = DateTime.Now;
        public DateTime? UpdDate { get; set; } = DateTime.Now;
    }
}
