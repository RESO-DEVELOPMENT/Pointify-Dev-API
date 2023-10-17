using ApplicationCore.Services;
using AutoMapper;
using Infrastructure.DTOs;
using Infrastructure.Models;
using Infrastructure.Repository;
using Infrastructure.UnitOfWork;

namespace ApplicationCore.Services
{
    public class MemberLevelMappingService : BaseService<MemberLevelMapping, MemberLevelMappingDto>, IMemberLevelMappingService
    {
        
        public MemberLevelMappingService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        protected override IGenericRepository<MemberLevelMapping> _repository => _unitOfWork.MemberLevelMappingRepository;

    }
}
