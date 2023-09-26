using System;

namespace Infrastructure.DTOs.MembershipLevel
{
    public class MembershipLevelDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool? DelFlag { get; set; }
        public int? MaxPoint { get; set; }
        public Guid? MembershipCardId { get; set; }
        public string Status { get; set; }
        public double? PointRedeemRate { get; set; }
        public int? LevelRank { get; set; }
    }

    public class MembershipLevelModel
    {
        public string Name { get; set; }
        public bool? DelFlag { get; set; }
        public int? MaxPoint { get; set; }
        public string Status { get; set; }
        public double? PointRedeemRate { get; set; }
        public int? LevelRank { get; set; }
    }
}
