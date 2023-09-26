using AutoMapper;
using Infrastructure.DTOs;
using Infrastructure.Models;
using Infrastructure.Repository;
using Infrastructure.UnitOfWork;

namespace ApplicationCore.Services
{
    public class StoreGameCampaignMappingService : BaseService<StoreGameCampaignMapping, StoreGameCampaignMappingDto>, IStoreGameCampaignMappingService
    {
        public StoreGameCampaignMappingService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {

        }
        protected override IGenericRepository<StoreGameCampaignMapping> _repository => _unitOfWork.StoreGameCampaignMappingRepository;
    }
}
