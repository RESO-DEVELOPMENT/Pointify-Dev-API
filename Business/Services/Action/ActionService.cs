

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
    public class ActionService : BaseService<Infrastructure.Models.Action, ActionDto>, IActionService
    {
        public ActionService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        protected override IGenericRepository<Infrastructure.Models.Action> _repository => _unitOfWork.ActionRepository;

        public async Task<bool> Delete(Infrastructure.Models.Action entity)
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

        public async Task<ActionDto> GetActionDetail(Guid id)
        {
            var result = new ActionDto();
            var entity = await _repository.GetFirst(filter: o => o.ActionId.Equals(id), includeProperties: "ActionProductMapping");
            if (entity != null)
            {
                result = _mapper.Map<ActionDto>(entity);
                result.ListProduct = new List<ActionProductMap>();
                result.ListProductMapp = new List<ActionProductMapping>();
                if (entity.ActionProductMapping.Count > 0)
                {
                    foreach (var product in entity.ActionProductMapping)
                    {
                        var dto = new ActionProductMap()
                        {
                            ProductId = product.ProductId,
                            Quantity = (int)product.Quantity,
                            Id = product.Id,
                        };
                        result.ListProduct.Add(dto);
                    }
                    result.ListProductMapp = entity.ActionProductMapping.ToList();
                }

            }
            return result;

        }

        public async Task<ActionDto> MyAddAction(ActionDto dto)
        {
            try
            {
                dto.ActionId = Guid.NewGuid();
                dto.InsDate = DateTime.Now;
                dto.UpdDate = DateTime.Now;
                var entity = _mapper.Map<Infrastructure.Models.Action>(dto);
                _repository.Add(entity);
                if (dto.ListProduct.Count > 0 && dto.ActionType > (int)AppConstant.EnvVar.ActionType.Shipping)
                {
                    IGenericRepository<ActionProductMapping> mappRepo = _unitOfWork.ActionProductMappingRepository;
                    foreach (var product in dto.ListProduct)
                    {
                        var mapp = new ActionProductMapping()
                        {
                            ActionId = dto.ActionId,
                            Id = Guid.NewGuid(),
                            InsDate = DateTime.Now,
                            UpdDate = DateTime.Now,
                            ProductId = product.ProductId,
                            Quantity = product.Quantity,
                        };
                        mappRepo.Add(mapp);
                    }
                }
                await _unitOfWork.SaveAsync();
                return _mapper.Map<ActionDto>(entity);

            }
            catch (System.Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                Debug.WriteLine(e.InnerException);
                throw new ErrorObj(code: (int)HttpStatusCode.InternalServerError, message: e.Message, description: AppConstant.ErrMessage.Internal_Server_Error);
            }

        }

        public async Task<ActionDto> UpdateAction(ActionDto dto)
        {
            var result = new ActionDto();
            var listProduct = dto.ListProduct;
            if (listProduct != null && listProduct.Count() > 0)
            {
                await UpdateActionMapp(listMapp: listProduct, actionId: dto.ActionId);
            }
            dto.ListProduct = null;
            dto.ListProductMapp = null;
            var entityMapp = _mapper.Map<Infrastructure.Models.Action>(dto);
            entityMapp.UpdDate = Common.GetCurrentDatetime();
            _repository.Update(entityMapp);
            await _unitOfWork.SaveAsync();
            result = _mapper.Map<ActionDto>(entityMapp);
            return result;
        }

        private async Task<bool> UpdateActionMapp(List<ActionProductMap> listMapp, Guid actionId)
        {
            try
            {
                IGenericRepository<ActionProductMapping> mappRepo = _unitOfWork.ActionProductMappingRepository;
                mappRepo.Delete(id: Guid.Empty, filter: el => el.ActionId.Equals(actionId));
                await _unitOfWork.SaveAsync();
                if (listMapp.Count > 0)
                {
                    var now = Common.GetCurrentDatetime();
                    foreach (var mapp in listMapp)
                    {
                        var entity = new ActionProductMapping()
                        {
                            Id = Guid.NewGuid(),
                            ActionId = actionId,
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
