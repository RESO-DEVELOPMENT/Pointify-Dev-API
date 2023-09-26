using System;
using System.Collections.Generic;

namespace Infrastructure.Models
{
    public partial class WalletType
    {
        public WalletType()
        {
            MemberWallet = new HashSet<MemberWallet>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid MemberShipProgramId { get; set; }
        public bool? DelFlag { get; set; }

        public virtual MembershipProgram MemberShipProgram { get; set; }
        public virtual ICollection<MemberWallet> MemberWallet { get; set; }
    }
}
