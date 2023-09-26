using System;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.DTOs
{
    public class DeviceDto : BaseDto
    {

        public Guid DeviceId { get; set; }
        public Guid StoreId { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
        [StringLength(8)]
        public string Code { get; set; }
        public Guid? GameCampaignId { get; set; }
    }

  
}
