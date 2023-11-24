using Infrastructure.Models;
using System.Collections.Generic;
using System;

namespace Infrastructure.DTOs.Membership
{
    public class MembershipResponse
    {
        public MembershipResponse()
        {
            MemberWallet = new HashSet<MemberWallet>();
            MembershipCard = new HashSet<MembershipCard>();
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
        public string NextLevelName { get; set; }

        public virtual MemberLevel MemberLevel { get; set; }
        public virtual Models.MembershipProgram MemberProgram { get; set; }
        public virtual ICollection<MemberWallet> MemberWallet { get; set; }
        public virtual ICollection<MembershipCard> MembershipCard { get; set; }
        public virtual ICollection<VoucherWallet> VoucherWallet { get; set; }
    }
}
