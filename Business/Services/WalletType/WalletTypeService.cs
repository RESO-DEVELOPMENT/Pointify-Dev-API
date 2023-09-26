using AutoMapper;
using Infrastructure.DTOs.WalletType;
using Infrastructure.Models;
using Infrastructure.Repository;
using Infrastructure.UnitOfWork;

namespace ApplicationCore.Services
{
    public class WalletTypeService : BaseService<WalletType, WalletTypeDto>, IWalletTypeService
    {
        public WalletTypeService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        protected override IGenericRepository<WalletType> _repository => _unitOfWork.WalletTypeRepository;
    }
}
