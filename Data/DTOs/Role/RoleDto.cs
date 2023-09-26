using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.DTOs
{
    public class RoleDto : BaseDto
    {
        public int RoleId { get; set; }
        public string Name { get; set; }
    }
}
