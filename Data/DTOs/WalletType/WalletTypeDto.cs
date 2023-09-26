using System;

namespace Infrastructure.DTOs.WalletType
{
    public class WalletTypeDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid MemberShipProgramId { get; set; }
        public bool? DelFlag { get; set; }
    }
}
