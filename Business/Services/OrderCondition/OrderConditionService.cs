
using AutoMapper;
using Infrastructure.DTOs;
using Infrastructure.Models;
using Infrastructure.Repository;
using Infrastructure.UnitOfWork;
namespace ApplicationCore.Services
{
    public class OrderConditionService : BaseService<OrderCondition, OrderConditionDto>, IOrderConditionService
    {
        public OrderConditionService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        protected override IGenericRepository<OrderCondition> _repository => _unitOfWork.OrderConditionRepository;
    }
}
