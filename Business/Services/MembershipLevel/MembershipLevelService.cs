using AutoMapper;
using Infrastructure.DTOs.MembershipLevel;
using Infrastructure.Models;
using Infrastructure.Repository;
using Infrastructure.UnitOfWork;

namespace ApplicationCore.Services
{
    public class MembershipLevelService : BaseService<MembershipLevel, MembershipLevelDto>, IMembershipLevelService
    {
        public MembershipLevelService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
        protected override IGenericRepository<MembershipLevel> _repository => _unitOfWork.MembershipLevelRepository;
    }
}
