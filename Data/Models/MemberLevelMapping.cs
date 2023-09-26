using System;
using System.Collections.Generic;

namespace Infrastructure.Models
{
    public partial class MemberLevelMapping
    {
        public Guid Id { get; set; }
        public Guid MemberLevelId { get; set; }
        public Guid PromotionId { get; set; }
        public DateTime InsDate { get; set; }
        public DateTime UpdDate { get; set; }

        public virtual MemberLevel MemberLevel { get; set; }
        public virtual Promotion Promotion { get; set; }
    }
}
