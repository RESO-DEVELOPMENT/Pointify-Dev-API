using System;
using System.Collections.Generic;

namespace Infrastructure.Models
{
    public partial class MemberWallet
    {
        public MemberWallet()
        {
            MemberAction = new HashSet<MemberAction>();
            Transaction = new HashSet<Transaction>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool DelFlag { get; set; }
        public Guid MemberId { get; set; }
        public Guid WalletTypeId { get; set; }
        public decimal Balance { get; set; }
        public decimal BalanceHistory { get; set; }

        public virtual Membership Member { get; set; }
        public virtual WalletType WalletType { get; set; }
        public virtual ICollection<MemberAction> MemberAction { get; set; }
        public virtual ICollection<Transaction> Transaction { get; set; }
    }
}
