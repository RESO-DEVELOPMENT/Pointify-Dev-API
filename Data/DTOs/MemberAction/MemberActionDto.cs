using System;

namespace Infrastructure.DTOs.MemberAction
{
    public class MemberActionDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public int? ActionType { get; set; }
        public decimal? ActionValue { get; set; }
        public int? Status { get; set; }
        public string Note { get; set; }
        public bool DelFlag { get; set; } = false;

        public Guid? MemberWalletId { get; set; }
        public Guid? MemberActionTypeId { get; set; }
        public Guid? MemberShipCardId { get; set; }
    }
    public class MemberActionModel
    {
        public string Name { get; set; }
        public int? ActionType { get; set; }
        public decimal? ActionValue { get; set; }
        public int? Status { get; set; }
        public string Note { get; set; }
    }
}
