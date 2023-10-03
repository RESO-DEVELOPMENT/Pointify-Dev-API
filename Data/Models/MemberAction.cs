﻿using System;
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
        public double? ActionValue { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public bool? DelFlag { get; set; }
        public Guid? MemberWalletId { get; set; }
        public Guid? MemberActionTypeId { get; set; }
<<<<<<< Updated upstream
        public Guid? MemberShipCardId { get; set; }
=======
        public DateTime? InsDate { get; set; }
        public DateTime? UpdDate { get; set; }
>>>>>>> Stashed changes

        public virtual MemberActionType MemberActionType { get; set; }
        public virtual MembershipCard MemberShipCard { get; set; }
        public virtual MemberWallet MemberWallet { get; set; }
        public virtual ICollection<Transaction> Transaction { get; set; }
    }
}
