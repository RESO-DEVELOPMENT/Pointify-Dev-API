using Infrastructure.DTOs;
using Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApplicationCore.Services
{
    public interface IMemberService : IBaseService<Member, MemberDto>
    {
        public Task<MemberDto> CreateMember(MemberDto dto);
        public Task<Member> GetMemberDetail(Guid id);
        public Task<Member> GetMemberProgram(Guid id, Guid program);

        public Task<MemberDto> UpdateMember(MemberDto dto);
    }
}
