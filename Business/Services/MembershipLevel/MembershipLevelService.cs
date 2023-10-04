using AutoMapper;
using Infrastructure.DTOs;
using Infrastructure.DTOs.MembershipLevel;
using Infrastructure.Helper;
using Infrastructure.Models;
using Infrastructure.Repository;
using Infrastructure.UnitOfWork;
using System.Net;
using System.Threading.Tasks;
using System;

namespace ApplicationCore.Services
{
    public class MembershipLevelService : BaseService<MembershipLevel, MembershipLevelDto>, IMembershipLevelService
    {
        public MembershipLevelService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
        protected override IGenericRepository<MembershipLevel> _repository => _unitOfWork.MembershipLevelRepository;

        //public async Task<MembershipLevel> GetMemberLevelByIdKey(Guid? id, Guid? apiKey)
        //{
        //    if (id.Equals(Guid.Empty) || id == null || apiKey == null)
        //    {
        //        throw new ErrorObj(code: (int)HttpStatusCode.BadRequest,
        //            message: AppConstant.ErrMessage.ApiKey_Not_Exist,
        //            description: AppConstant.ErrMessage.ApiKey_Not_Exist);
        //    }

        //    try
        //    {
        //        var result = await _repository.GetFirst(filter: o =>
        //                o.Id.Equals(id)
        //                && o.Bra.Equals(apiKey),
        //            includeProperties: "MemberLevel,MemberProgram,MemberWallet");
        //        return result;
        //    }
        //    catch (ErrorObj e)
        //    {
        //        throw e;
        //    }
        //}
    }
}
