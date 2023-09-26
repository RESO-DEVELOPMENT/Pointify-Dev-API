using System;
using System.Collections.Generic;

namespace Infrastructure.Models
{
    public partial class MembershipCardType
    {
        public MembershipCardType()
        {
            MembershipCard = new HashSet<MembershipCard>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? AppendCode { get; set; }
        public Guid MemberShipProgramId { get; set; }
        public bool Active { get; set; }
        public string CardImg { get; set; }

        public virtual MembershipProgram MemberShipProgram { get; set; }
        public virtual ICollection<MembershipCard> MembershipCard { get; set; }
    }
}
