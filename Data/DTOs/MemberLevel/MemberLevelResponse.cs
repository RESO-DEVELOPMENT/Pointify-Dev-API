using Infrastructure.Models;
using System;
using System.Collections.Generic;

namespace Infrastructure.DTOs.MemberLevel
{
    public class MemberLevelResponse
    {
        public Guid MemberLevelId { get; set; }
        public string Name { get; set; }
        public int? IndexLevel { get; set; }
        public string Benefits { get; set; }
        public int? MaxPoint { get; set; }
        public string NextLevelName { get; set; }
        public virtual ICollection<MemberWalletResponse> MemberWallet { get; set; }
        public virtual ICollection<CardResponse> MembershipCard { get; set; }
    }
    public class MemberWalletResponse
    {
        public Guid Id { get; set; }
        public decimal Balance { get; set; }
        public WalletTypeResponse WalletType { get; set; }
    }
    public class WalletTypeResponse
    {
        public string Name { get; set; }
        public string Currency { get; set; }
    }
    public class CardResponse
    {
        public Guid Id { get; set; }
        public string MembershipCardCode { get; set; }
        public string PhysicalCardCode { get; set; }
        public CardType MembershipCardType { get; set; }
    }
    public class CardType
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string cardImage { get; set; }
    }
    public class VoucherWalletReponse
    {
        public string Status { get; set; }
        public DateTime RedeemDate { get; set; }
        public Guid VoucherId { get; set; }
    }
}
