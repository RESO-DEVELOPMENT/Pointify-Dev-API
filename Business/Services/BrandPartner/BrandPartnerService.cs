using AutoMapper;
using Infrastructure.DTOs;
using Infrastructure.DTOs.BrandPartner;
using Infrastructure.Models;
using Infrastructure.Repository;
using Infrastructure.UnitOfWork;
using System;
using System.Threading.Tasks;

namespace ApplicationCore.Services
{
    public class BrandPartnerService : BaseService<BrandPartner, BrandPartnerDto>, IBrandPartnerService
    {
        public BrandPartnerService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        protected override IGenericRepository<Infrastructure.Models.BrandPartner> _repository => _unitOfWork.BrandPartnerRepository;

        public async Task<BrandPartner> CreateBrandPartner(BrandPartnerDto dto)
        {
            BrandPartner entity = new BrandPartner()
            {
                Id = Guid.NewGuid(),
                BrandId = dto.BrandId,
                BrandPartnerId = dto.BrandPartnerId,
                DeFlag = false,
                InsDate = DateTime.Now,
                PartnerBalance = dto.PartnerBalance,
                UpdDate = DateTime.Now
            };
            _repository.Add(entity);
            await _unitOfWork.SaveAsync();
            return entity;
        }

        public async Task<BrandPartner> DeleteBrandPartner(Guid id)
        {
            var entity = await _repository.GetFirst(filter: o => o.Id.Equals(id) && !o.DeFlag);
            if (entity == null)
            {
                return null;
            }
            entity.DeFlag = true;
            _repository.Update(entity);
            await _unitOfWork.SaveAsync();
            return entity;
        }

        public async Task<BrandPartner> getBrandPartner(Guid id)
        {
            var entity = await _repository.GetFirst(filter: o => o.Id.Equals(id) && !o.DeFlag);
            if (entity == null)
            {
                return null;
            }
            return entity;
        }

    }
}
