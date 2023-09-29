using System;
using System.Collections.Generic;

namespace Infrastructure.Models
{
    public partial class WalletType
    {
        public WalletType()
        {
            MemberActionType = new HashSet<MemberActionType>();
            MemberWallet = new HashSet<MemberWallet>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid MemberShipProgramId { get; set; }
        public bool? DelFlag { get; set; }
        public string Currency { get; set; }

        public virtual MembershipProgram MemberShipProgram { get; set; }
        public virtual ICollection<MemberActionType> MemberActionType { get; set; }
        public virtual ICollection<MemberWallet> MemberWallet { get; set; }
    }
}
