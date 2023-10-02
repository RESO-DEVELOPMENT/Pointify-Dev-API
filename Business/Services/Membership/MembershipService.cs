using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
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

        IGenericRepository<WalletType> _wallet => _unitOfWork.WalletTypeRepository;
        IGenericRepository<MemberWallet> _memberWallet => _unitOfWork.MemberWalletRepository;
        
        public async Task<MembershipDto> CreateNewMember(Guid apiKey, MembershipDto dto)
        {
            try
            {
                var lowestLevel = await _level.GetFirst(filter: o =>
                    !o.DelFlg
                    && o.BrandId.Equals(apiKey) && o.IndexLevel == 0);
                var listWallet = (await _wallet.Get(filter: o =>
                    !o.DelFlag ?? true
                    && o.MemberShipProgramId.Equals(dto.MemberProgramId))).ToList();
                dto.MembershipId = Guid.NewGuid();
                dto.InsDate = DateTime.Now;
                dto.UpdDate = DateTime.Now;
                dto.DelFlg = false;
                dto.MemberLevelId = lowestLevel.MemberLevelId;
                var entity = _mapper.Map<Membership>(dto);
                _repository.Add(entity);
                List<MemberWallet> memberWallets = new List<MemberWallet>();
                foreach (var walletType in listWallet)
                {
                    MemberWallet memberWallet = new MemberWallet()
                    {
                        Id = new Guid(),
                        Balance = 0,
                        BalanceHistory = 0,
                        DelFlag = false,
                        MemberId = dto.MembershipId,
                        Name = walletType.Name,
                        WalletTypeId = walletType.Id
                    };
                    _memberWallet.Add(memberWallet);
                }

                await _unitOfWork.SaveAsync();
                return _mapper.Map<MembershipDto>(entity);
            }
            catch (System.Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                Debug.WriteLine(e.InnerException);
                throw new ErrorObj(code: (int) HttpStatusCode.InternalServerError, message: e.Message,
                    description: AppConstant.ErrMessage.Internal_Server_Error);
            }
        }
    }
}