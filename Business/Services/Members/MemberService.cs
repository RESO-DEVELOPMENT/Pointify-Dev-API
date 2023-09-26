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
using static Infrastructure.Helper.AppConstant.NOTIFY_MESSAGE;

namespace ApplicationCore.Services
{
    public class MemberService : BaseService<Member, MemberDto>, IMemberService
    {
        public MemberService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        protected override IGenericRepository<Member> _repository => _unitOfWork.MemberRepository;
        IGenericRepository<MembershipProgram> _program => _unitOfWork.MemberShipProgramRepository;

        public async Task<MemberDto> CreateMember(MemberDto dto)
        {
            try
            {
                dto.Id = Guid.NewGuid();
                dto.InsDate = DateTime.Now;
                dto.UpdDate = DateTime.Now;
                var entity = _mapper.Map<Infrastructure.Models.Member>(dto);
                _repository.Add(entity);
                IGenericRepository<Member> mappRepo = _unitOfWork.MemberRepository;
                var mapp = new Member
                {
                    Id = Guid.NewGuid(),
                    InsDate = DateTime.Now,
                    UpdDate = DateTime.Now,
                    DelFlg = false
                };
                mappRepo.Add(mapp);
                await _unitOfWork.SaveAsync();
                return _mapper.Map<MemberDto>(entity);
            }
            catch (System.Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                Debug.WriteLine(e.InnerException);
                throw new ErrorObj(code: (int)HttpStatusCode.InternalServerError, message: e.Message, description: AppConstant.ErrMessage.Internal_Server_Error);
            }
        }

        public async Task<Member> GetMemberDetail(Guid id)
        {
            var result = new MemberDto();
            var entity = await _repository.GetFirst(
                filter: o => o.Id.Equals(id),
                includeProperties: "MembershipCard,MemberWallet");
            return entity;
        }

        public async Task<Member> GetMemberProgram(Guid id, Guid program)
        {

            var entity = await _repository.GetFirst(
                filter: o => o.Id.Equals(id) && o.MemberProgramId.Equals(program));
            var pro = await _program.GetFirst(filter: o =>  o.Id.Equals(entity.MemberProgramId));
            return entity;
        }

        public async Task<MemberDto> UpdateMember(MemberDto dto)
        {
            try
            {
                var result = _mapper.Map<Member>(dto);
                var entity = await _repository.GetFirst(filter: o => o.Id.Equals(result.Id));                                         
                if (entity != null)
                {
                    entity.UpdDate = DateTime.Now;
                    if (dto.FullName != null)
                    {
                        entity.FullName = result.FullName;
                    }
                    if (dto.PhoneNumber != null)
                    {
                        entity.PhoneNumber = result.PhoneNumber;
                    }
                    if (entity.CustomerId == null)
                    {
                        entity.CustomerId = result.CustomerId;
                    }
                    if (!dto.MemberProgramId.Equals(Guid.Empty) && !dto.MemberProgramId.Equals(entity.MemberProgramId))
                    {
                        IGenericRepository<MembershipProgram> membership = (IGenericRepository<MembershipProgram>)_unitOfWork.MemberShipProgramRepository;
                        var oldMem = await membership.GetFirst(filter: o => o.Id.Equals(entity.Id) && (bool)!o.DelFlg);
                        if (oldMem != null)
                        {
                            oldMem.Member.Remove(entity);
                        }
                        var newMem = await membership.GetFirst(filter: o => o.Id.Equals(result.MemberProgramId) && (bool)!o.DelFlg);
                        newMem.Member.Add(entity);
                        entity.MemberProgramId = result.MemberProgramId;

                    }  
                }
                entity.DelFlg = result.DelFlg;
                _repository.Update(entity);
                await _unitOfWork.SaveAsync();
                return _mapper.Map<MemberDto>(entity);
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
