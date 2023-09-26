
using AutoMapper;
using Infrastructure.DTOs;
using Infrastructure.Models;
using Infrastructure.Repository;
using Infrastructure.UnitOfWork;

namespace ApplicationCore.Services
{
    public class ProductConditionService : BaseService<ProductCondition, ProductConditionDto>, IProductConditionService
    {
        public ProductConditionService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        protected override IGenericRepository<ProductCondition> _repository => _unitOfWork.ProductConditionRepository;
    }
}
