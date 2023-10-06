using Infrastructure.DTOs.BrandPartner;
using Infrastructure.Models;
using System;
using System.Threading.Tasks;

namespace ApplicationCore.Services
{
    public interface IBrandPartnerService : IBaseService<BrandPartner, BrandPartnerDto>
    {
        public Task<BrandPartner> CreateNewBrandPartner(BrandPartnerDto dto);
        public  Task<string> DeleteBrandPartner(Guid id);
        public Task<BrandPartner> GetBrandPartnerById(Guid id);
        public Task<BrandPartner> UpdateBalancePartner(Guid ApiKey, UpdateBalance balance);
    }
}
