using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.DTOs
{
    public class VoucherParamResponse
    {
        public VoucherParamResponse(Guid voucherGroupId, string voucherGroupName, Guid voucherId, string code, string description)
        {
            VoucherGroupId = voucherGroupId;
            VoucherGroupName = voucherGroupName;
            VoucherId = voucherId;
            Code = code;
            Description = description;
        }

       public Guid VoucherGroupId { get; set; }
       public string VoucherGroupName { get; set; }
       public Guid VoucherId { get; set; }
       public string Code { get; set; }

       public string Description { get; set; }
}
}
