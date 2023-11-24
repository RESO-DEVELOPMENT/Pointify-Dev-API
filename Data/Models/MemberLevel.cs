using System;
using System.Collections.Generic;

namespace Infrastructure.Models
{
    public partial class MemberLevel
    {
        public MemberLevel()
        {
            MemberLevelMapping = new HashSet<MemberLevelMapping>();
            Membership = new HashSet<Membership>();
        }

        public Guid MemberLevelId { get; set; }
        public Guid BrandId { get; set; }
        public string Name { get; set; }
        public bool DelFlg { get; set; }
        public DateTime UpdDate { get; set; }
        public DateTime InsDate { get; set; }
        public int? IndexLevel { get; set; }
        public string Benefits { get; set; }
        public int? MaxPoint { get; set; }

        public virtual Brand Brand { get; set; }
        public virtual ICollection<MemberLevelMapping> MemberLevelMapping { get; set; }
        public virtual ICollection<Membership> Membership { get; set; }
    }
}
