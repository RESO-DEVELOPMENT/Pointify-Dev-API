using Infrastructure.DTOs;
using Infrastructure.Helper;
using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationCore.Services
{
    public interface IProductService : IBaseService<Product,ProductDto>
    {
        public Task<bool> CheckExistin(string code, Guid brandId, Guid productId);
        public Task<ProductDto> Update(ProductDto dto);

        //public Task<ProductSyncParamDTO> CallSyncProduct(Guid brandId, ProductRequestParam productRequestParam);
        public Task<GenericRespones<BrandProductDto>> GetBrandProduct(int PageSize, int PageIndex, Guid brandId);
        public Task<List<BrandProductDto>> GetAllBrandProduct(Guid brandId);
        public Task<ProductSyncParamDTO> SyncProduct(Guid brandId, ProductRequestParam productRequestParam);
        public Task<PromotionInfomation> CheckGiftProduct(ProductDto dto);
    }
}
