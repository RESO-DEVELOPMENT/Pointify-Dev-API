using ApplicationCore.Utils;
using AutoMapper;
using Infrastructure.DTOs;
using Infrastructure.Helper;
using Infrastructure.Models;
using Infrastructure.Repository;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace ApplicationCore.Services
{
    public class MemberShipCardService : BaseService<MembershipCard, MemberShipCardDto>, IMemberShipCardService
    {
        private readonly IBrandService _brandService;

        public MemberShipCardService(IUnitOfWork unitOfWork, IMapper mapper, IBrandService brandService) : base(unitOfWork, mapper)
        {
            _brandService = brandService;
        }

        protected override IGenericRepository<MembershipCard> _repository => _unitOfWork.MemberShipCardRepository;



        public async Task<MemberShipCardDto> CreateMemberShipCard(MemberShipCardDto dto)
        {
            try
            {
                var check = await _brandService.GetFirst(filter: el => el.BrandId.Equals(dto.BrandId));
                if (check == null) { return null; }
                dto.Id = Guid.NewGuid();
                //dto.MembershipCardCode = Common.makeCode(10);
                var digit = Common.makeCode(10);
                var checkCard = await _repository.GetFirst(filter: o => o.MembershipCardCode == digit);
                while(checkCard != null)
                {
                    digit = Common.makeCode(10);
                    checkCard = await _repository.GetFirst(filter: o => o.MembershipCardCode == digit);
                }
                dto.MembershipCardCode = digit;
                dto.Active = true;
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

        public Task<MemberShipCardDto> GetMemberShipCardDetail(Guid id, Guid apiKey)
        {
            throw new NotImplementedException();
        }

        public async Task<MemberShipCardDto> UpdateMemberShipCard(Guid id, MemberShipCardDto dto, Guid apiKey)
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
                var result = await _repository.GetFirst(filter: o => o.Id.Equals(id)
                                                        && o.BrandId.Equals(apiKey));
                if (result == null)
                {
                    throw new ErrorObj(code: (int)HttpStatusCode.NotFound,
                        message: AppConstant.ErrMessage.ApiKey_Not_Exist,
                        description: AppConstant.ErrMessage.ApiKey_Not_Exist);
                }

                result.MemberId = dto.MemberId;
                result.MembershipCardCode = dto.MembershipCardCode;
                result.PhysicalCardCode = dto.PhysicalCardCode;
                _repository.Update(result);
                await _unitOfWork.SaveAsync();
                return _mapper.Map<MemberShipCardDto>(result);
            }
            catch (ErrorObj e)
            {
                throw e;
            }
        }

        public async Task<bool> AddCodeForMember(Guid id, Guid apiKey)
        {
            try
            {
                var check = false;
                var result = await _repository.GetFirst(filter: o => o.Id.Equals(id)
                                                        && o.BrandId.Equals(apiKey));
                if (result == null)
                {
                    throw new ErrorObj(code: (int)HttpStatusCode.NotFound,
                        message: AppConstant.ErrMessage.ApiKey_Not_Exist,
                        description: AppConstant.ErrMessage.ApiKey_Not_Exist);
                }

                result.PhysicalCardCode = result.MembershipCardCode;
                _repository.Update(result);
                await _unitOfWork.SaveAsync();
                check = true;
                return check;
            }
            catch (ErrorObj e)
            {
                throw e;
            }
        }
    }
}
