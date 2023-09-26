using System;
using System.Collections.Generic;

namespace Infrastructure.Models
{
    public partial class MembershipProgram
    {
        public MembershipProgram()
        {
            Member = new HashSet<Member>();
            MemberActionType = new HashSet<MemberActionType>();
            MembershipCardType = new HashSet<MembershipCardType>();
            WalletType = new HashSet<WalletType>();
        }

        public Guid Id { get; set; }
        public Guid BrandId { get; set; }
        public string NameOfProgram { get; set; }
        public DateTime? StartDay { get; set; }
        public DateTime? EndDay { get; set; }
        public bool? DelFlg { get; set; }
        public string TermAndConditions { get; set; }
        public string Status { get; set; }

        public virtual Brand Brand { get; set; }
        public virtual ICollection<Member> Member { get; set; }
        public virtual ICollection<MemberActionType> MemberActionType { get; set; }
        public virtual ICollection<MembershipCardType> MembershipCardType { get; set; }
        public virtual ICollection<WalletType> WalletType { get; set; }
    }
}
