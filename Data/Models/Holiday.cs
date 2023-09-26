using System;
using System.Collections.Generic;

namespace Infrastructure.Models
{
    public partial class Holiday
    {
        public Guid HolidayId { get; set; }
        public string HolidayName { get; set; }
        public DateTime Date { get; set; }
        public bool DelFlg { get; set; }
        public DateTime InsDate { get; set; }
        public DateTime UpdDate { get; set; }
    }
}
