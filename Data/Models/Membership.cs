using System;
using System.Collections.Generic;

namespace Infrastructure.Models
{
    public partial class Membership
    {
        public Membership()
        {
            MemberWallet = new HashSet<MemberWallet>();
            MembershipCard = new HashSet<MembershipCard>();
            Voucher = new HashSet<Voucher>();
            VoucherWallet = new HashSet<VoucherWallet>();
        }

        public Guid MembershipId { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Fullname { get; set; }
        public bool DelFlg { get; set; }
        public DateTime InsDate { get; set; }
        public DateTime UpdDate { get; set; }
        public Guid MemberProgramId { get; set; }
        public Guid? MemberLevelId { get; set; }
        public int? Gender { get; set; }

        public virtual MemberLevel MemberLevel { get; set; }
        public virtual MembershipProgram MemberProgram { get; set; }
        public virtual ICollection<MemberWallet> MemberWallet { get; set; }
        public virtual ICollection<MembershipCard> MembershipCard { get; set; }
        public virtual ICollection<Voucher> Voucher { get; set; }
        public virtual ICollection<VoucherWallet> VoucherWallet { get; set; }
    }
}
