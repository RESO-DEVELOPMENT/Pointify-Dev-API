using AutoMapper;
using Infrastructure.DTOs.MemberAction;
using Infrastructure.Models;
using Infrastructure.Repository;
using Infrastructure.UnitOfWork;

namespace ApplicationCore.Services
{
    public class MemberActionService : BaseService<MemberAction, MemberActionDto>, IMemberActionService
    {
        public MemberActionService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
        protected override IGenericRepository<MemberAction> _repository => _unitOfWork.MemberActionRepository;
    }
}
