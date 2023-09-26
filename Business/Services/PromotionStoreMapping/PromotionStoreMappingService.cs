using Infrastructure.Models;
using System;
using Infrastructure.DTOs;
using Infrastructure.Repository;
using Infrastructure.UnitOfWork;
using AutoMapper;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace ApplicationCore.Services
{
    public class PromotionStoreMappingService : BaseService<PromotionStoreMapping, PromotionStoreMappingDto>, IPromotionStoreMappingService
    {
        public PromotionStoreMappingService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {

        }

        protected override IGenericRepository<PromotionStoreMapping> _repository => _unitOfWork.PromotionStoreMappingRepository;

        public async Task<bool> DeletePromotionStoreMapping(Guid promotionId)
        {
            _repository.Delete(promotionId, filter: el => el.PromotionId.Equals(promotionId));
            return await _unitOfWork.SaveAsync() > 0;
        }

    }
}
