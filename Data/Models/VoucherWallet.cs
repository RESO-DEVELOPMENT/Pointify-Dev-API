using System;
using System.Collections.Generic;

namespace Infrastructure.Models
{
    public partial class VoucherWallet
    {
        public Guid Id { get; set; }
        public Guid MemberId { get; set; }
        public Guid? OrderId { get; set; }
        public string Status { get; set; }
        public DateTime RedeemDate { get; set; }
        public Guid? VoucherId { get; set; }
    }
}
