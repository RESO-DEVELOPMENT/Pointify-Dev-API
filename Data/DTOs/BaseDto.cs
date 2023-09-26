using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.DTOs
{
    public class BaseDto
    {
        public bool DelFlg { get; set; } = false;
        public DateTime? InsDate { get; set; } = DateTime.Now;
        public DateTime? UpdDate { get; set; } = DateTime.Now;
    }
}
