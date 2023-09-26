using System;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.DTOs
{
    public class BrandDeviceDto
    {
        public Guid DeviceId { get; set; }
        public Guid StoreId { get; set; }
        public string StoreCode { get; set; }
        public string StoreName { get; set; }
        public decimal? Group { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
        [StringLength(8)]
        public string Code { get; set; }
        public Guid? GameConfigId { get; set; }
        public string GameConfigName { get; set; }
    }
}
