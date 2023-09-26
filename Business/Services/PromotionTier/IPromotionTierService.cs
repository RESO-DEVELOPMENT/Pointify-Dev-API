
using Infrastructure.DTOs;
using Infrastructure.Helper;
using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationCore.Services
{
    public interface IPromotionTierService : IBaseService<PromotionTier, PromotionTierDto>
    {
        public Task<GenericRespones<AvailableDto>> GetAvailable(int pageSize, int pageIndex, Guid brandId, string actionType, string discountType);
        public Task<bool> AssignTierToPromo(Guid promotionId, Guid promotionTierId);
        public Task<PromotionTierDto> CreateTier(PromotionTierDto dto);
        public Task<bool> DeleteTier(Guid id);
        public Task<bool> UpdateTier(PromotionTierDto dto, PromotionTier entity);
        public Task<Promotion> GetPromotionTierByProductCode(string productCode);
        public Task<PromotionTierDto> CreateGiftTier(PromotionTierDto dto);
    }
}
