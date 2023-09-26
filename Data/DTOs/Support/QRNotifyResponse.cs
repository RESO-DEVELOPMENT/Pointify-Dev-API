using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.DTOs
{
    public class QRNotifyResponse : PayResponse
    {
        public string partnerRefId { get; set; }
        public string momoTransId { get; set; }
        public string message { get; set; }
    }
}
