using System;
using System.Collections.Generic;

namespace Infrastructure.Models
{
    public partial class Membership
    {
        public Membership()
        {
            Voucher = new HashSet<Voucher>();
        }

        public Guid MembershipId { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Fullname { get; set; }
        public bool DelFlg { get; set; }
        public DateTime InsDate { get; set; }
        public DateTime UpdDate { get; set; }

        public virtual ICollection<Voucher> Voucher { get; set; }
    }
}
