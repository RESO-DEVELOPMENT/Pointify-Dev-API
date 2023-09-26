using AutoMapper;
using Infrastructure.DTOs.MembershipProgram;
using Infrastructure.Models;
using Infrastructure.Repository;
using Infrastructure.UnitOfWork;

namespace ApplicationCore.Services
{
    public class MembershipProgramService : BaseService<MembershipProgram, MembershipProgramDto>, IMembershipProgramService
    {
        public MembershipProgramService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        protected override IGenericRepository<MembershipProgram> _repository => _unitOfWork.MembershipProgramRepository;
    }
}
