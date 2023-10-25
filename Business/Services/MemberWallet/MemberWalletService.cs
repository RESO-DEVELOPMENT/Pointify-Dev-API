using AutoMapper;
using Infrastructure.DTOs;
using Infrastructure.Helper;
using Infrastructure.Models;
using Infrastructure.Repository;
using Infrastructure.UnitOfWork;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;

namespace ApplicationCore.Services
{
    public class MemberWalletService : BaseService<MemberWallet, MemberWalletDto>, IMemberWalletService
    {
        public MemberWalletService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        protected override IGenericRepository<MemberWallet> _repository => _unitOfWork.MemberWalletRepository;

        //protected IGenericRepository<Member> _members => _unitOfWork.MemberRepository;
        private IMembershipService _member;
        //protected IGenericRepository<WalletType> _wallet => _unitOfWork.Wall

        public async Task<MemberWallet> GetMemberWalletByIdKey(Guid? id, Guid? apiKey)
        {
            if (id.Equals(Guid.Empty) || id == null || apiKey == null)
            {
                throw new ErrorObj(code: (int)HttpStatusCode.BadRequest,
                    message: AppConstant.ErrMessage.ApiKey_Not_Exist,
                    description: AppConstant.ErrMessage.ApiKey_Not_Exist);
            }

            try
            {
                var result = await _repository.GetFirst(filter: o =>
                        (bool)!o.DelFlag
                        && o.Id.Equals(id)
                        && o.WalletType.MemberShipProgram.BrandId.Equals(apiKey),
                    includeProperties: "MemberAction,Transaction,WalletType");
                return result;
            }
            catch (ErrorObj e)
            {
                throw e;
            }
        }
        public async Task<MemberWalletDto> CreateWallet(MemberWalletDto dto)
        {
            try
            {
                dto.Id = Guid.NewGuid();
                dto.DelFlag = false;
                dto.Balance = 0;
                var entity = _mapper.Map<MemberWallet>(dto);
                _repository.Add(entity);
                //WallType == null
                await _unitOfWork.SaveAsync();
                return _mapper.Map<MemberWalletDto>(entity);
            }
            catch (System.Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                Debug.WriteLine(e.InnerException);
                throw new ErrorObj(code: (int) HttpStatusCode.InternalServerError, message: e.Message,
                    description: AppConstant.ErrMessage.Internal_Server_Error);
            }
        }

        public async Task<bool> HideWallet(Guid id, string value)
        {
            _repository.Hide(id, value);
            return await _unitOfWork.SaveAsync() > 0;
        }

        public async Task<MemberWallet> GetMemberWalletById(Guid id)
        {
            //check id
            if (id.Equals(Guid.Empty))
            {
                throw new ErrorObj(code: (int) HttpStatusCode.BadRequest,
                    message: AppConstant.ErrMessage.ApiKey_Not_Exist,
                    description: AppConstant.ErrMessage.ApiKey_Not_Exist);
            }

            try
            {
                var result = await _repository.GetFirst(filter: o => o.Id.Equals(id));
                return result;
            }
            catch (ErrorObj e)
            {
                throw e;
            }
        }

        public async Task<MemberWalletDto> UpdateWallet(Guid id, UpMemberWallet dto, Guid apiKey)
        {
            //check id
            if (id.Equals(Guid.Empty))
            {
                throw new ErrorObj(code: (int) HttpStatusCode.BadRequest,
                    message: AppConstant.ErrMessage.ApiKey_Not_Exist,
                    description: AppConstant.ErrMessage.ApiKey_Not_Exist);
            }

            try
            {
                var result = await _repository.GetFirst(filter: o => o.Id.Equals(id)
                                                        && o.WalletType.MemberShipProgram.BrandId.Equals(apiKey));
                if (result == null)
                {
                    throw new ErrorObj(code: (int) HttpStatusCode.NotFound,
                        message: AppConstant.ErrMessage.ApiKey_Not_Exist,
                        description: AppConstant.ErrMessage.ApiKey_Not_Exist);
                }

                result.Name = dto.Name;
                result.Balance = dto.Balance;
                result.BalanceHistory = dto.BalanceHistory;
                _repository.Update(result);
                await _unitOfWork.SaveAsync();
                return _mapper.Map<MemberWalletDto>(result);
            }
            catch (ErrorObj e)
            {
                throw e;
            }
        }

        public async Task<string> DeleteWallet(Guid id)
        {
            //check id
            if (id.Equals(Guid.Empty))
            {
                throw new ErrorObj(code: (int) HttpStatusCode.BadRequest,
                    message: AppConstant.ErrMessage.ApiKey_Not_Exist,
                    description: AppConstant.ErrMessage.ApiKey_Not_Exist);
            }

            try
            {
                var result = await _repository.GetFirst(filter: o => o.Id.Equals(id));
                if (result == null)
                {
                    throw new ErrorObj(code: (int) HttpStatusCode.NotFound,
                        message: AppConstant.ErrMessage.ApiKey_Not_Exist,
                        description: AppConstant.ErrMessage.ApiKey_Not_Exist);
                }

                result.DelFlag = true;
                _repository.Update(result);
                await _unitOfWork.SaveAsync();
                return "Bạn đã xoá MemberWallet thành công";
            }
            catch (ErrorObj e)
            {
                return "Bạn đã xoá MemberWallet thất bại";
            }
        }
    }
}