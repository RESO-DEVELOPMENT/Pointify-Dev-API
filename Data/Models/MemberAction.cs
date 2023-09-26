using System;
using System.Collections.Generic;

namespace Infrastructure.Models
{
    public partial class MemberAction
    {
        public MemberAction()
        {
            Transaction = new HashSet<Transaction>();
        }

        public Guid Id { get; set; }
        public decimal? ActionValue { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public bool? DelFlag { get; set; }
        public Guid? MemberWalletId { get; set; }
        public Guid? MemberActionTypeId { get; set; }
        public Guid? MemberShipCardId { get; set; }

        public virtual MemberActionType MemberActionType { get; set; }
        public virtual MembershipCard MemberShipCard { get; set; }
        public virtual MemberWallet MemberWallet { get; set; }
        public virtual ICollection<Transaction> Transaction { get; set; }
    }
}
