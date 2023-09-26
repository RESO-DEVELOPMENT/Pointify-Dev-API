
using Infrastructure.DTOs;
using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApplicationCore.Services
{
    public interface IGiftService : IBaseService<Gift, GiftDto>
    {
        public Task<GiftDto> MyAddAction(GiftDto dto);
        public Task<bool> Delete(Gift entity);
        public Task<GiftDto> GetGiftDetail(Guid id);
        public Task<GiftDto> UpdateGift(GiftDto dto);

    }
}
