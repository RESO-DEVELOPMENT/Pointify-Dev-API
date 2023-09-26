using Infrastructure.DTOs;
using Infrastructure.DTOs.VoucherChannel;
using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationCore.Services
{
   public interface IVoucherChannelService : IBaseService<PromotionChannelMapping, VoucherChannelDto>
    {
        
    }
}
