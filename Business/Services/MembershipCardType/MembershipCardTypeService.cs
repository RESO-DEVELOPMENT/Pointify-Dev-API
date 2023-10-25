using ApplicationCore.Services;
using AutoMapper;
using Infrastructure.DTOs;
using Infrastructure.Models;
using Infrastructure.Repository;
using Infrastructure.UnitOfWork;

namespace ApplicationCore.Services
{
    public class MembershipCardTypeService : BaseService<MembershipCardType, MembershipCardTypeDto>, IMembershipCardTypeService
    {
        public MembershipCardTypeService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        protected override IGenericRepository<MembershipCardType> _repository => _unitOfWork.MembershipCardTypeResponsitory;
    }
}
