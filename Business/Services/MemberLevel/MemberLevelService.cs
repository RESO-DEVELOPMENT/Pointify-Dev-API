using AutoMapper;
using Infrastructure.DTOs;
using Infrastructure.Helper;
using Infrastructure.Models;
using Infrastructure.Repository;
using Infrastructure.UnitOfWork;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ApplicationCore.Services
{
    public class MemberLevelService : BaseService<MemberLevel, MemberLevelDto>, IMemberLevelService
    {
        public MemberLevelService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        protected override IGenericRepository<MemberLevel> _repository => _unitOfWork.MemberLevelRepository;

        public async Task<bool> CheckExistingLevel(string name, Guid brandId, Guid memberLevelId)
        {
            try
            {
                var isExist = false;
                if (!memberLevelId.Equals(Guid.Empty))
                {
                    isExist = (await _repository.Get(filter: o => o.Name.ToLower().Equals(name)
                        && !o.DelFlg
                        && !o.MemberLevelId.Equals(memberLevelId)
                        && o.BrandId.Equals(brandId))).ToList().Count > 0;
                }
                else
                {
                    isExist = (await _repository.Get(filter: o => o.Name.ToLower().Equals(name)
                        && !o.DelFlg
                        && o.BrandId.Equals(brandId))).ToList().Count > 0;
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

        public async Task<MemberLevelDto> Update(MemberLevelDto dto)
        {
            try
            {
                var entity = await _repository.GetFirst(filter: o => o.MemberLevelId.Equals(dto.MemberLevelId)
                && !o.DelFlg);
                if (entity != null)
                {
                    entity.UpdDate = DateTime.Now;
                    var updParam = _mapper.Map<MemberLevel>(dto);
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
                    return _mapper.Map<MemberLevelDto>(entity);
                }
                else
                {
                    throw new ErrorObj(code: (int)AppConstant.ErrCode.MemberLevel_NotFound, message:AppConstant.ErrMessage.MemberLevel_NotFound);
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
