﻿using System;
using System.Collections.Generic;

namespace Infrastructure.Models
{
    public partial class MembershipCard
    {
        public Guid Id { get; set; }
        public Guid MemberId { get; set; }
        public string MembershipCardCode { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedTime { get; set; }
        public Guid BrandId { get; set; }
        public Guid MembershipCardTypeId { get; set; }
        public string PhysicalCardCode { get; set; }

        public virtual Membership Member { get; set; }
        public virtual MembershipCardType MembershipCardType { get; set; }
    }
}
