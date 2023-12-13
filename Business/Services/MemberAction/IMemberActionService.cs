using System;
using System.Threading.Tasks;
using Infrastructure.DTOs.MemberAction;
using Infrastructure.DTOs.Request;
using Infrastructure.Models;

namespace ApplicationCore.Services
{
    public interface IMemberActionService : IBaseService<MemberAction, MemberActionDto>
    {
        public Task<MemberActionDto> CreateMemberAction(MemberActionRequest request, Guid promotionId);
    }
}