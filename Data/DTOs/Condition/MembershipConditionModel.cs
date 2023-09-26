using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationCore.Models
{
    public class MembershipConditionModel : ConditionModel
    {
        public bool ForNewMember { get; set; }
        public string MembershipLevel { get; set; }
    }
}
