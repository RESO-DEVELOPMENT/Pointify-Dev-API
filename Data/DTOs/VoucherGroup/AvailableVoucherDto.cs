using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.DTOs
{
    public class AvailableVoucherDto
    {
        public Guid VoucherGroupId { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
    }
}
