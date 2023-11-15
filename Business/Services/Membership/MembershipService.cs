using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ApplicationCore.Utils;
using AutoMapper;
using Infrastructure.DTOs;
using Infrastructure.Helper;
using Infrastructure.Models;
using Infrastructure.Repository;
using Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Parameters;

namespace ApplicationCore.Services
{
    public class MembershipService : BaseService<Membership, MembershipDto>, IMembershipService
    {
        public MembershipService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        protected override IGenericRepository<Membership> _repository => _unitOfWork.MembershipRepository;
        IGenericRepository<MemberLevel> _level => _unitOfWork.MemberLevelRepository;
        IGenericRepository<MembershipProgram> _program => _unitOfWork.MembershipProgramRepository;
        IGenericRepository<WalletType> _wallet => _unitOfWork.WalletTypeRepository;
        IGenericRepository<MemberWallet> _memberWallet => _unitOfWork.MemberWalletRepository;
        IGenericRepository<MembershipCard> _membershipCard => _unitOfWork.MemberShipCardRepository;
        IGenericRepository<MembershipCardType> _membershipCardType => _unitOfWork.MembershipCardTypeResponsitory;


        //done
        public async Task<MembershipDto> CreateNewMember(Guid apiKey, MembershipDto dto)
        {
            try
            {
                var lowestLevel = await _level.GetFirst(filter: o =>
                    !o.DelFlg
                    && o.BrandId.Equals(apiKey) && o.IndexLevel == 0);
                var listWallet = (await _wallet.Get(filter: o =>
                    !o.DelFlag
                    && o.MemberShipProgramId.Equals(dto.MemberProgramId))).ToList();
                if (dto.MembershipId == null || dto.MembershipId.Equals(System.Guid.Empty))
                {
                    dto.MembershipId = Guid.NewGuid();
                }
                dto.InsDate = DateTime.Now;
                dto.UpdDate = DateTime.Now;
                dto.DelFlg = false;
                dto.MemberLevelId = lowestLevel.MemberLevelId;
                var entity = _mapper.Map<Membership>(dto);
                _repository.Add(entity);
                foreach (var walletType in listWallet)
                {
                    MemberWallet memberWallet = new MemberWallet()
                    {
                        Id = Guid.NewGuid(),
                        Balance = 0,
                        BalanceHistory = 0,
                        DelFlag = false,
                        MemberId = dto.MembershipId,
                        Name = walletType.Name,
                        WalletTypeId = walletType.Id
                    };
                    _memberWallet.Add(memberWallet);
                }

                //Create MembershipCard - MembershipCardType
                var digit = Common.makeCode(10);
                var checkCard = await _membershipCard.GetFirst(filter: o => o.MembershipCardCode == digit);
                while (checkCard != null)
                {
                    digit = Common.makeCode(10);
                    checkCard = await _membershipCard.GetFirst(filter: o => o.MembershipCardCode == digit);
                }
                MemberShipCardDto membershipCard = new MemberShipCardDto()
                {
                    Id = Guid.NewGuid(),
                    MemberId = dto.MembershipId,
                    BrandId = lowestLevel.BrandId,
                    MembershipCardCode = digit,
                    Active = true,
                    CreatedTime = DateTime.Now,

                };
                MembershipCardType membershipCardType = new MembershipCardType()
                {
                    Id = Guid.NewGuid(),
                    Name = "Normal",
                    AppendCode = Guid.NewGuid(),
                    Active = true,
                    MemberShipProgramId = dto.MemberProgramId
                };
                _membershipCardType.Add(membershipCardType);

                membershipCard.MembershipCardTypeId = membershipCardType.Id;
                var membershipCardItem = _mapper.Map<MembershipCard>(membershipCard);
                _membershipCard.Add(membershipCardItem);
                await _unitOfWork.SaveAsync();
                return _mapper.Map<MembershipDto>(entity);
            }
            catch (System.Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                Debug.WriteLine(e.InnerException);
                throw new ErrorObj(code: (int)HttpStatusCode.InternalServerError, message: e.Message,
                    description: AppConstant.ErrMessage.Internal_Server_Error);
            }
        }

        //Done
        public async Task<Membership> GetMembershipByIdKey(Guid? id, Guid? apiKey)
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
                        !o.DelFlg
                        && o.MembershipId.Equals(id)
                        && o.MemberProgram.BrandId.Equals(apiKey),
                    includeProperties: "MemberLevel,MemberProgram,MemberWallet,MembershipCard");
                return result;
            }
            catch (ErrorObj e)
            {
                throw e;
            }
        }
        //Done
        public async Task<Membership> GetMembershipById(Guid? id)
        {
            //check id
            if (id.Equals(Guid.Empty) || id == null)
            {
                throw new ErrorObj(code: (int)HttpStatusCode.BadRequest,
                    message: AppConstant.ErrMessage.ApiKey_Not_Exist,
                    description: AppConstant.ErrMessage.ApiKey_Not_Exist);
            }

            try
            {
                var result = await _repository.GetFirst(filter: o =>
                        !o.DelFlg
                        && o.MembershipId.Equals(id),
                    includeProperties: "MemberLevel,MemberProgram,MemberWallet,MembershipCard");
                return result;
            }
            catch (ErrorObj e)
            {
                throw e;
            }
        }

        //done
        public async Task<string> DeleteMembership(Guid id)
        {
            //check id
            if (id.Equals(Guid.Empty))
            {
                throw new ErrorObj(code: (int)HttpStatusCode.BadRequest,
                    message: AppConstant.ErrMessage.ApiKey_Not_Exist,
                    description: AppConstant.ErrMessage.ApiKey_Not_Exist);
            }

            try
            {
                var result = await _repository.GetFirst(filter: o => o.MembershipId.Equals(id));
                if (result == null)
                {
                    throw new ErrorObj(code: (int)HttpStatusCode.NotFound,
                        message: AppConstant.ErrMessage.ApiKey_Not_Exist,
                        description: AppConstant.ErrMessage.ApiKey_Not_Exist);
                }

                result.DelFlg = true;
                _repository.Update(result);
                await _unitOfWork.SaveAsync();
                return "Bạn đã xoá Membership thành công";
            }
            catch (ErrorObj e)
            {
                return "Bạn đã xoá Membership thất bại";
            }
        }

        public async Task<MembershipDto> UpdateMemberShip(Guid id, UpMembership update, Guid apiKey)
        {
            //check id
            if (id.Equals(Guid.Empty))
            {
                throw new ErrorObj(code: (int)HttpStatusCode.BadRequest,
                    message: AppConstant.ErrMessage.ApiKey_Not_Exist,
                    description: AppConstant.ErrMessage.ApiKey_Not_Exist);
            }

            try
            {
                var result = await _repository.GetFirst(filter: o => o.MembershipId.Equals(id) && !o.DelFlg
                                                                && o.MemberProgram.BrandId.Equals(apiKey));
                if (result == null)
                {
                    throw new ErrorObj(code: (int)HttpStatusCode.NotFound,
                        message: AppConstant.ErrMessage.ApiKey_Not_Exist,
                        description: AppConstant.ErrMessage.ApiKey_Not_Exist);
                }

                result.PhoneNumber = update.PhoneNumber;
                result.Email = update.Email;
                result.Fullname = update.Fullname;
                result.InsDate = DateTime.Now;
                result.UpdDate = DateTime.Now;
                _repository.Update(result);
                await _unitOfWork.SaveAsync();
                return _mapper.Map<MembershipDto>(result);
            }
            catch (ErrorObj e)
            {
                throw e;
            }
        }
    }
}