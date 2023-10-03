using System;

namespace Infrastructure.DTOs.Request
{
    public class MemberActionRequest
    {
        public Guid ApiKey { get; set; }
        public string StoreCode { get; set; }
        public Guid MembershipId { get; set; }
        public decimal Amount { get; set; }
        public string MemberActionType { get; set; }
        public string Description { get; set; }

        public MemberActionRequest(Guid apiKey, string storeCode, Guid membershipId, decimal amount,
            string memberActionType, string description)
        {
            ApiKey = apiKey;
            this.StoreCode = storeCode;
            this.MembershipId = membershipId;
            this.Amount = amount;
            this.MemberActionType = memberActionType;
            this.Description = description;
        }
    }
}