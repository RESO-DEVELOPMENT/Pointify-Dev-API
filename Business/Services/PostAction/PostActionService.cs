using ApplicationCore.Utils;
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
    public class GiftService : BaseService<Gift, GiftDto>, IGiftService
    {
        public GiftService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        protected override IGenericRepository<Gift> _repository => _unitOfWork.GiftRepository;

        public async Task<bool> Delete(Gift entity)
        {
            try
            {
                entity.DelFlg = true;
                _repository.Update(entity);
                return await _unitOfWork.SaveAsync() > 0;
            }

            catch (System.Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                Debug.WriteLine(e.InnerException);
                throw new ErrorObj(code: (int)HttpStatusCode.InternalServerError, message: e.Message, description: AppConstant.ErrMessage.Internal_Server_Error);
            }
        }

        public async Task<GiftDto> GetGiftDetail(Guid id)
        {
            var result = new GiftDto();
            var entity = await _repository.GetFirst(filter: o => o.GiftId.Equals(id), includeProperties: "GiftProductMapping");
            if (entity != null)
            {
                result = _mapper.Map<GiftDto>(entity);
                result.ListProductMapp = new List<GiftProductMapping>();
                if (entity.GiftProductMapping.Count > 0)
                {
                    result.ListProductMapp = entity.GiftProductMapping.ToList();
                }
            }
            return result;
        }

        public async Task<GiftDto> MyAddAction(GiftDto dto)
        {
            try
            {
                dto.GiftId = Guid.NewGuid();
                dto.InsDate = DateTime.Now;
                dto.UpdDate = DateTime.Now;
                var entity = _mapper.Map<Gift>(dto);
                _repository.Add(entity);
                if (dto.PostActionType == (int)AppConstant.EnvVar.PostActionType.Gift_Product && dto.ListProduct.Count > 0)
                {
                    IGenericRepository<GiftProductMapping> mappRepo = _unitOfWork.GiftProductMappingRepository;
                    foreach (var product in dto.ListProduct)
                    {
                        var mapp = new GiftProductMapping()
                        {
                            Id = Guid.NewGuid(),
                            GiftId = dto.GiftId,
                            ProductId = product.ProductId,
                            InsDate = DateTime.Now,
                            UpdDate = DateTime.Now,
                            Quantity = product.Quantity,

                        };
                        mappRepo.Add(mapp);
                    }
                }
                await _unitOfWork.SaveAsync();
                return _mapper.Map<GiftDto>(entity);
            }
            catch (System.Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                Debug.WriteLine(e.InnerException);
                throw new ErrorObj(code: (int)HttpStatusCode.InternalServerError, message: e.Message, description: AppConstant.ErrMessage.Internal_Server_Error);
            }

        }

        public async Task<GiftDto> UpdateGift(GiftDto dto)
        {
            try
            {
                var result = new GiftDto();
                var id = dto.GiftId;
                var listProduct = dto.ListProduct;
                if (listProduct != null && listProduct.Count() > 0)
                {
                    await UpdateGiftMapp(listMapp: listProduct, giftId: dto.GiftId);
                }

                dto.ListProduct = null;
                dto.ListProductMapp = null;
                var entityMapp = _mapper.Map<Gift>(dto);
                if (!dto.GameCampaignId.Equals(Guid.Empty))
                {
                    IGenericRepository<GameCampaign> gameRepo = _unitOfWork.GameConfigRepository;
                    var game = await gameRepo.GetFirst(filter: el => el.Id.Equals(dto.GameCampaignId));
                    if (game != null)
                    {
                        entityMapp.GameCampaign = game;
                    }
                }

                entityMapp.UpdDate = Common.GetCurrentDatetime();
                _repository.Update(entityMapp);
                await _unitOfWork.SaveAsync();
                result = _mapper.Map<GiftDto>(entityMapp);
                return result;
            }
            catch (System.Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                Debug.WriteLine(e.InnerException);
                throw new ErrorObj(code: (int)HttpStatusCode.InternalServerError, message: e.Message);
            }

        }

        private async Task<bool> UpdateGiftMapp(List<GiftProductMapp> listMapp, Guid giftId)
        {
            try
            {
                IGenericRepository<GiftProductMapping> mappRepo = _unitOfWork.GiftProductMappingRepository;
                mappRepo.Delete(id: Guid.Empty, filter: el => el.GiftId.Equals(giftId));
                await _unitOfWork.SaveAsync();
                if (listMapp.Count > 0)
                {
                    var now = Common.GetCurrentDatetime();
                    foreach (var mapp in listMapp)
                    {
                        var entity = new GiftProductMapping()
                        {
                            Id = Guid.NewGuid(),
                            GiftId = giftId,
                            ProductId = mapp.ProductId,
                            Quantity = mapp.Quantity,
                            InsDate = now,
                            UpdDate = now,
                        };
                        mappRepo.Add(entity);
                    }
                }
                return await _unitOfWork.SaveAsync() > 0;
            }
            catch (System.Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                Debug.WriteLine(e.InnerException);
                throw new ErrorObj(code: (int)HttpStatusCode.InternalServerError, message: e.Message);
            }

        }

    }
}
