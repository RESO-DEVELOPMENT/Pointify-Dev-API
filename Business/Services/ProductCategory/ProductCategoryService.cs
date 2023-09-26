using AutoMapper;
using Infrastructure.DTOs;
using Infrastructure.Helper;
using Infrastructure.Models;
using Infrastructure.Repository;
using Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ApplicationCore.Services
{
    public class ProductCategoryService : BaseService<ProductCategory, ProductCategoryDto>, IProductCateService
    {
        public ProductCategoryService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        protected override IGenericRepository<ProductCategory> _repository => _unitOfWork.ProductCategoryRepository;

        public async Task<bool> CheckExistin(string cateId, Guid brandId, Guid productCateId)
        {
            try
            {
                var isExist = false;
                if (!productCateId.Equals(Guid.Empty))
                {
                    isExist = (await _repository.Get(filter: o => o.CateId.Equals(cateId.ToString())
                        && o.BrandId.Equals(brandId)
                        && !o.ProductCateId.Equals(productCateId)
                        && !o.DelFlg)).ToList().Count > 0;
                }
                else
                {
                    isExist = (await _repository.Get(filter: o => o.CateId.Equals(cateId.ToString())
                        && o.BrandId.Equals(brandId) 
                        && !o.DelFlg)).ToList().Count > 0;
                }

                return isExist;
            }
            catch (Exception e)
            {
                //chạy bằng debug mode để xem log
                Debug.WriteLine("\n\nError at getVoucherForGame: \n" + e.Message);
                throw new ErrorObj(code: (int)HttpStatusCode.InternalServerError, message: e.Message);
            }

        }

        public async Task<List<ProductCategoryDto>> GetAll(Guid brandId)
        {
            try
            {
                var entity = await _repository.Get(filter: o => o.BrandId.Equals(brandId) && !o.DelFlg);
                if (entity != null)
                {
                    return _mapper.Map<List<ProductCategoryDto>>(entity);
                }
                return new List<ProductCategoryDto>();

            }
            catch (Exception e)
            {
                //chạy bằng debug mode để xem log
                Debug.WriteLine("\n\nError at getVoucherForGame: \n" + e.Message);
                throw new ErrorObj(code: (int)HttpStatusCode.InternalServerError, message: e.Message);
            }
        }

        public async Task<ProductCategoryDto> Update(ProductCategoryDto dto)
        {
            try
            {
                var entity = await _repository.GetFirst(filter: o => o.ProductCateId.Equals(dto.ProductCateId) && !o.DelFlg);
                if (entity != null)
                {
                    entity.UpdDate = DateTime.Now;
                    var updParam = _mapper.Map<ProductCategory>(dto);
                    if (updParam.CateId != null)
                    {
                        entity.CateId = dto.CateId;
                    }
                    if (updParam.Name != null)
                    {
                        entity.Name = dto.Name;
                    }
                    if (updParam.DelFlg != null)
                    {
                        entity.DelFlg = dto.DelFlg;
                    }
                    _repository.Update(entity);
                    await _unitOfWork.SaveAsync();
                    return _mapper.Map<ProductCategoryDto>(entity);
                }
                else
                {
                    throw new ErrorObj(code: (int)AppConstant.ErrCode.Product_Cate_NotFound, message: AppConstant.ErrMessage.Product_Cate_NotFound);
                }
            }
            catch (Exception e)
            {
                //chạy bằng debug mode để xem log
                Debug.WriteLine("\n\nError at getVoucherForGame: \n" + e.Message);
                throw new ErrorObj(code: (int)HttpStatusCode.InternalServerError, message: e.Message);
            }
        }
    }
}
