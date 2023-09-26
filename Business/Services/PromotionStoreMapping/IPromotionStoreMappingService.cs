
using Infrastructure.DTOs;
using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ApplicationCore.Services
{
    public interface IPromotionStoreMappingService : IBaseService<PromotionStoreMapping,PromotionStoreMappingDto>
    {
        public Task<bool> DeletePromotionStoreMapping(Guid promotionId);
    }
}
