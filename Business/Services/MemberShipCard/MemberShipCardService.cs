using ApplicationCore.Utils;
using AutoMapper;
using Infrastructure.DTOs;
using Infrastructure.Helper;
using Infrastructure.Models;
using Infrastructure.Repository;
using Infrastructure.UnitOfWork;
using System;
using System.Diagnostics;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace ApplicationCore.Services
{
    public class MemberShipCardService : BaseService<MembershipCard, MemberShipCardDto>, IMemberShipCardService
    {
        public MemberShipCardService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        protected override IGenericRepository<MembershipCard> _repository => _unitOfWork.MemberShipCardRepository;

        public async Task<MemberShipCardDto> CreateMemberShipCard(MemberShipCardDto dto)
        {
            try
            {
                dto.Id = Guid.NewGuid();
                //dto.MembershipCardCode = Common.makeCode(10);
                var digit = Common.makeCode(10);
                var checkCard = await _repository.GetFirst(filter: o => o.MembershipCardCode == digit);
                if (checkCard == null)
                {
                    dto.MembershipCardCode = digit;
                }
                dto.Active = false;
                dto.CreatedTime = DateTime.Now;
                var entity = _mapper.Map<MembershipCard>(dto);
                _repository.Add(entity);
                await _unitOfWork.SaveAsync();
                return _mapper.Map<MemberShipCardDto>(entity);
            }
            catch (System.Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                Debug.WriteLine(e.InnerException);
                throw new ErrorObj(code: (int)HttpStatusCode.InternalServerError, message: e.Message, description: AppConstant.ErrMessage.Internal_Server_Error);
            }

        }

        public Task<bool> DeleteMemberShipCard(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<MemberShipCardDto> GetMemberShipCardDetail(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<MemberShipCardDto> UpdateMemberShipCard(MemberShipCardDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
