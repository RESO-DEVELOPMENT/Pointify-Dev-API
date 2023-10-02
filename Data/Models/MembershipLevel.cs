using System;
using System.Collections.Generic;

namespace Infrastructure.Models
{
    public partial class MembershipLevel
    {
        public MembershipLevel()
        {
            Member = new HashSet<Member>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool? DelFlag { get; set; }
        public int? MaxPoint { get; set; }
        public Guid ProgramId { get; set; }
        public string Status { get; set; }
        public double? PointRedeemRate { get; set; }
        public int Index { get; set; }

        public virtual MembershipProgram Program { get; set; }
        public virtual ICollection<Member> Member { get; set; }
    }
}
