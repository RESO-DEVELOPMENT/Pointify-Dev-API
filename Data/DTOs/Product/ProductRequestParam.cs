using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.DTOs
{
    public class ProductRequestParam
    {
        public string TokenUrl { get; set; }
        public TokenBody TokenBody { get; set; }
        public string SyncUrl { get; set; }
    }
    public class TokenBody
    {
        public string UserName { get; set; }
        public string PassWord { get; set; }
        public int Device_Id { get; set; }
    }
}
