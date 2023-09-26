using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.DTOs
{
    public class ChannelDto : BaseDto
    {
        public Guid ChannelId { get; set; }
        public Guid? BrandId { get; set; }
        public string ChannelName { get; set; }
        public string ChannelCode { get; set; }
        public decimal Group { get; set; }
        public int ChannelType { get; set; }
        public string ApiKey { get; set; }
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }

    }
}
