﻿using System;
using System.Threading.Tasks;
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
            MemberActionType actionType = await _memberActionType
                .GetFirst(
                    filter: x => x.Code.Equals(request.MemberActionType));

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
                InsDate = DateTime.Now,
                UpdDate = DateTime.Now,
            };
            _repository.Add(memberAction);
            await _unitOfWork.SaveAsync();
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
            Guid guid = Guid.Empty;
            if (memberAction.Status == MemberActionStatus.Success)
            {
                Transaction transaction = new Transaction()
                {
                    Id = Guid.NewGuid(),
                    BrandId = request.ApiKey,
                    InsDate = DateTime.Now,
                    UpdDate = DateTime.Now,
                    MemberActionId = memberAction.Id,
                    MemberWalletId = memberAction.MemberWalletId,
                    TransactionJson = memberAction.Description,
                    Amount = memberAction.ActionValue,
                    Currency = wallet.WalletType.Currency,
                    Type = request.MemberActionType,
                    IsIncrease = (request.MemberActionType == TrasactionType.GET_POINT 
                    || request.MemberActionType == TrasactionType.TOP_UP) ? true : false,
                };
                guid = transaction.Id;
                _transaction.Add(transaction);
                var isSuccess = await _unitOfWork.SaveAsync();
                if (isSuccess < 1)
                {
                    memberAction.Status = MemberActionStatus.Fail;
                    memberAction.Description = $"[{MemberActionStatus.Fail}] Giao dịch thất bại";
                }
            }
            memberAction.UpdDate = DateTime.Now;
            _memberWallet.Update(wallet);
            _repository.Update(memberAction);
            var isSuccessful = await _unitOfWork.SaveAsync();
            var response = _mapper.Map<MemberActionDto>(memberAction);
            response.TransactionId = guid;
            return isSuccessful > 0 ? response : null;
        }
    }
}