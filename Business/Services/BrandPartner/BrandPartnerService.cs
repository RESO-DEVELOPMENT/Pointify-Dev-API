using AutoMapper;
using Infrastructure.DTOs.BrandPartner;
using Infrastructure.Helper;
using Infrastructure.Models;
using Infrastructure.Repository;
using Infrastructure.UnitOfWork;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Threading.Tasks;
using static Infrastructure.Helper.AppConstant.EnvVar;

namespace ApplicationCore.Services
{
    public class BrandPartnerService : BaseService<BrandPartner, BrandPartnerDto>, IBrandPartnerService
    {
        public BrandPartnerService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        protected override IGenericRepository<BrandPartner> _repository => _unitOfWork.BrandPartnerRepository;

        public async Task<BrandPartner> CreateNewBrandPartner(BrandPartnerDto dto)
        {
            BrandPartner brandPartner = new BrandPartner()
            {
                Id = Guid.NewGuid(),
                BrandId = dto.BrandId,
                PartnerBalance = 0,
                BrandPartnerId = dto.BrandPartnerId,
                InsDate = DateTime.Now,
                UpdDate = DateTime.Now,
                DeFlag = false
            };
            _repository.Add(brandPartner);
            await _unitOfWork.SaveAsync();
            return brandPartner;
        }

        public async Task<string> DeleteBrandPartner(Guid id)
        {
            BrandPartner brandPartner = await _repository.GetFirst(filter: o => o.Id.Equals(id) && !o.DeFlag);
            if (brandPartner == null)
            {
                return "BrandPartner not found";
            }
            brandPartner.DeFlag = true;
            _repository.Update(brandPartner);
            await _unitOfWork.SaveAsync();
            return "Delete success";
        }
        public async Task<BrandPartner> GetBrandPartnerById (Guid id)
        {
            BrandPartner brandPartner = await _repository.GetById(id);
            if(brandPartner == null)
            {
                return null;
            }
            return brandPartner;
        }

        public async Task<BrandPartner> UpdateBalance (Guid ApiKey,UpdateBalance balance)
        {
            var brandPartner = await _repository.GetFirst(filter: o => o.BrandId.Equals(ApiKey) &&
            o.BrandPartnerId.Equals(balance.BrandPartnerId) && !o.DeFlag);
            brandPartner.PartnerBalance = brandPartner.PartnerBalance + balance.PartnerBalance;
        }
    }
}
