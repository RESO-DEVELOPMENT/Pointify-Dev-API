using AutoMapper;
using Infrastructure.DTOs.MemberActionType;
using Infrastructure.Models;
using Infrastructure.Repository;
using Infrastructure.UnitOfWork;

namespace ApplicationCore.Services
{
    public class MemberActionTypeService : BaseService<MemberActionType, MemberActionTypeDto>, IMemberActionTypeService
    {
        public MemberActionTypeService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
        protected override IGenericRepository<MemberActionType> _repository => _unitOfWork.MemberActionTypeRepository;
    }
}
