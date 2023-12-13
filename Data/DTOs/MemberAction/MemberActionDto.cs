using System;

namespace Infrastructure.DTOs.MemberAction
{
        public class MemberActionDto
        {
                public Guid Id { get; set; }
                public decimal ActionValue { get; set; }
                public string Status { get; set; }
                public string Description { get; set; }
                public bool? DelFlag { get; set; }
                public Guid? MemberWalletId { get; set; }
                public Guid? MemberActionTypeId { get; set; }
                public Guid? TransactionId { get; set; }
        }


}