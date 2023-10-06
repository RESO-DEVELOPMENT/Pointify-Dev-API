using Infrastructure.DTOs.BrandPartner;
using Infrastructure.Models;
using System;
using System.Threading.Tasks;

namespace ApplicationCore.Services
{
    public interface IBrandPartnerService : IBaseService<Infrastructure.Models.BrandPartner, BrandPartnerDto>
    {
        public Task<BrandPartner> CreateBrandPartner(BrandPartnerDto dto);
        public Task<BrandPartner> DeleteBrandPartner(Guid id);
        public Task<BrandPartner> getBrandPartner(Guid id);
    }
}
