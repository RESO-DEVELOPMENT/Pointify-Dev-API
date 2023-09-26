using Infrastructure.DTOs;

using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationCore.Services
{
    public interface IStoreService : IBaseService<Store, StoreDto>

    {
        public Task<List<GroupStoreOfPromotion>> GetStoreOfPromotion(Guid promotionId, Guid brandId);
        public Task<List<GroupStoreOfPromotion>> UpdateStoreOfPromotion(UpdateStoreOfPromotion dto);

        public Task<List<PromotionInfomationJsonFile>> GetPromotionsForStore(string brandCode, string storeCode);
        public Stream Get(string brandCode, string storeCode);
    }
}
