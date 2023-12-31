﻿using System;
using System.Threading.Tasks;
using ApplicationCore.Utils;
using AutoMapper;
using Infrastructure.DTOs.MemberAction;
using Infrastructure.DTOs.Request;
using Infrastructure.Helper;
using Infrastructure.Models;
using Infrastructure.Repository;
using Infrastructure.UnitOfWork;
using static Infrastructure.Helper.AppConstant;

namespace ApplicationCore.Services
{
    public class MemberActionService : BaseService<MemberAction, MemberActionDto>, IMemberActionService
    {
        public MemberActionService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        protected override IGenericRepository<MemberAction> _repository => _unitOfWork.MemberActionRepository;

        protected IGenericRepository<MemberActionType> _memberActionType => _unitOfWork.MemberActionTypeRepository;
        protected IGenericRepository<MemberWallet> _memberWallet => _unitOfWork.MemberWalletRepository;
        protected IGenericRepository<Transaction> _transaction => _unitOfWork.TransactionRepository;


        public async Task<MemberActionDto> CreateMemberAction(MemberActionRequest request)
        {
            Membership membership = await _unitOfWork.MembershipRepository.GetFirst(
                               filter: x => x.MembershipId.Equals(request.MembershipId),
                                              includeProperties: "MemberProgram"
                                                         );
            MemberActionType actionType = await _memberActionType
                .GetFirst(
                    filter: x => x.Code.Equals(request.MemberActionType) && x.MemberShipProgramId.Equals(membership.MemberProgramId));

            MemberWallet wallet = await _memberWallet.GetFirst(
                filter: x =>
                    x.MemberId.Equals(request.MembershipId) && x.WalletTypeId.Equals(actionType.MemberWalletTypeId),
                "WalletType"
            );

            MemberAction memberAction = new MemberAction()
            {
                Id = Guid.NewGuid(),
                Description = request.Description,
                Status = MemberActionStatus.Prossecing,
                ActionValue = 0,
                MemberWalletId = wallet.Id,
                MemberActionTypeId = actionType.Id,
                InsDate = TimeUtils.GetCurrentSEATime(),
                UpdDate = TimeUtils.GetCurrentSEATime(),
            };
            _repository.Add(memberAction);
            await _unitOfWork.SaveAsync();
            MemberActionDto dto = new MemberActionDto()
            {
                Id = memberAction.Id,
                ActionValue = memberAction.ActionValue,
                Description = memberAction.Description,
                MemberWalletId = memberAction.MemberWalletId,
                MemberActionTypeId = memberAction.MemberActionTypeId
            };
            switch (request.MemberActionType)
            {
                case TrasactionType.GET_POINT:
                    {
                        wallet.Balance += request.Amount;
                        wallet.BalanceHistory += request.Amount;
                        memberAction.ActionValue = request.Amount;
                        memberAction.Status = MemberActionStatus.Success;
                        memberAction.Description = $"[{MemberActionStatus.Success}] " + request.Description;
                        break;
                    }
                case TrasactionType.TOP_UP:
                    {
                        wallet.Balance += request.Amount;
                        wallet.BalanceHistory += request.Amount;
                        memberAction.ActionValue = request.Amount;
                        memberAction.Status = MemberActionStatus.Success;
                        memberAction.Description = $"[{MemberActionStatus.Success}] " + request.Description;
                        break;
                    }
                case TrasactionType.PAYMENT:
                    {
                        if (wallet.Balance < request.Amount)
                        {
                            memberAction.Status = MemberActionStatus.Fail;
                            memberAction.Description = $"[{MemberActionStatus.Fail}] Số dư tài khoản không đủ";
                            break;
                        }

                        wallet.Balance -= request.Amount;
                        memberAction.ActionValue = request.Amount;
                        memberAction.Status = MemberActionStatus.Success;
                        memberAction.Description = $"[{MemberActionStatus.Success}] " + request.Description;
                        break;
                    }
            }
            if (memberAction.Status == MemberActionStatus.Success)
            {
                Transaction transaction = new Transaction()
                {
                    Id = Guid.NewGuid(),
                    BrandId = request.ApiKey,
                    InsDate = TimeUtils.GetCurrentSEATime(),
                    UpdDate = TimeUtils.GetCurrentSEATime(),
                    MemberActionId = memberAction.Id,
                    MemberWalletId = memberAction.MemberWalletId,
                    TransactionJson = memberAction.Description,
                    Amount = memberAction.ActionValue,
                    Currency = wallet.WalletType.Currency,
                    Type = request.MemberActionType,
                    IsIncrease =
                        (request.MemberActionType == AppConstant.EffectMessage.GetPoint ||
                         request.MemberActionType == AppConstant.EffectMessage.TopUp),
                };
                _transaction.Add(transaction);
                var isSuccess = await _unitOfWork.SaveAsync();
                if (isSuccess < 1)
                {
                    memberAction.Status = MemberActionStatus.Fail;
                    memberAction.Description = $"[{MemberActionStatus.Fail}] Giao dịch thất bại";
                }
                else
                {
                    dto.TransactionId = transaction.Id;
                }
            }

            memberAction.UpdDate = TimeUtils.GetCurrentSEATime();
            _memberWallet.Update(wallet);
            _repository.Update(memberAction);
            var isSuccessful = await _unitOfWork.SaveAsync();
            if (isSuccessful > 0)
            {
                dto.Description = memberAction.Description;
                dto.Status = memberAction.Status;
            }

            return isSuccessful > 0 ? dto : null;
        }
    }
}