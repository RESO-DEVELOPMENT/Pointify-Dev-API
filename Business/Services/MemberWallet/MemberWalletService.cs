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
        private IMemberService _member;
        //protected IGenericRepository<WalletType> _wallet => _unitOfWork.Wall


        public async Task<MemberWalletDto> CreateWallet(MemberWalletDto dto)
        {
            try
            {
                dto.Id = Guid.NewGuid();
                dto.DelFlag = false;
                dto.Balance = 0.0;
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
                throw new ErrorObj(code: (int)HttpStatusCode.InternalServerError, message: e.Message, description: AppConstant.ErrMessage.Internal_Server_Error);
            }
        }

        public async Task<bool> HideWallet(Guid id, string value)
        {
            _repository.Hide(id, value);
            return await _unitOfWork.SaveAsync() > 0;
        }

        public async Task<MemberWalletDto> GetWalletDetail(Guid id)
        {
            try
            {
                var result = await _repository.GetFirst(
                    el => el.Id.Equals(id)
                          && (bool)el.DelFlag,
                    includeProperties: "Transaction,MemberAction"
                );
                return _mapper.Map<MemberWalletDto>(result);
            }
            catch (ErrorObj e)
            {
                throw e;
            }
        }

        public Task<MemberWalletDto> UpdateWallet(MemberDto dto)
        {
            throw new NotImplementedException();
        }


    }
}
