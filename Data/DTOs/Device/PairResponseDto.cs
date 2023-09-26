using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.DTOs
{
    public class PairResponseDto
    {
        public string Token { get; set; }
        public string BrandCode { get; set; }
        public string StoreCode { get; set; }
        public string DeviceCode { get; set; }
        public string StoreName { get; set; }

        public Guid DeviceId { get; set; }
        public Guid BrandId { get; set; }
    }
}
