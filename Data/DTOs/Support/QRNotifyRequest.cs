using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.DTOs
{
    public class QRNotifyRequest : PayResponse
    {
        public string partnerRefId { get; set; }
        public string momoTransId { get; set; }
        public string message { get; set; }
        public string partnerCode { get; set; }
        public string accessKey { get; set; }
        public string partnerTransId { get; set; }
        public string transType { get; set; }
        public long responseTime { get; set; }
        public string storeId { get; set; }

    }
}
