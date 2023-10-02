using System;

namespace Infrastructure.DTOs.Request
{
    public class MemberActionRequest
    {
        public Guid ApiKey { get; set; }
        public string StoreCode { get; set; }
        public Guid MembershipId { get; set; }
        public int Amount { get; set; }
        public Guid MemberActionTypeId { get; set; }
        public string Description { get; set; }

        public MemberActionRequest(Guid apiKey, string storeCode, Guid membershipId, int amount,
            Guid actionTypeId, string description)
        {
            ApiKey = apiKey;
            this.StoreCode = storeCode;
            this.MembershipId = membershipId;
            this.Amount = amount;
            this.MemberActionTypeId = actionTypeId;
            this.Description = description;
        }
    }
}