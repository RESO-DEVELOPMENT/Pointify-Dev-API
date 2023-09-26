using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.DTOs
{
    public class VoucherForCustomerModel
    {
        public string CusFullName { get; set; }
        public string CusEmail { get; set; }
        public string CusPhoneNo { get; set; }
        public string CusGender { get; set; }
        public string ChannelCode { get; set; }

    }
    public class VoucherForOtherChannel
    {
        public string ApiKey { get; set; }
        public string ChannelCode { get; set; }
        public string BrandCode { get; set; }
        public string Hash { get; set; }
    }
}
