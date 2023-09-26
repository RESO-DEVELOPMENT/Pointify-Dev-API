using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.DTOs
{
    public class PayResponse
    {
        public int status { get; set; }
        public string signature { get; set; }
        public long amount { get; set; }
    }
}
