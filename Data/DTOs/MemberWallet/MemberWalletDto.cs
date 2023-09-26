using Infrastructure.Models;
using System;
using System.Collections.Generic;

namespace Infrastructure.DTOs
{
    public class MemberWalletDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool? DelFlag { get; set; }
        public Guid? MemberId { get; set; }
        public Guid? WalletTypeId { get; set; }
        public double Balance { get; set; }

        public int? BalanceHistory { get; set; }

    }
}
