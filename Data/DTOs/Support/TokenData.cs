using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.DTOs.Support
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Data
    {
        public string AccountId { get; set; }
        public object AccountPassword { get; set; }
        public bool IsUsed { get; set; }
        public object Role { get; set; }
        public object StaffName { get; set; }
        public int StoreId { get; set; }
        public string Token { get; set; }
    }

    public class TokenData
    {
        public Data data { get; set; }
        public string message { get; set; }
        public int status { get; set; }
        public bool success { get; set; }
    }


}
