using Infrastructure.DTOs;
using Infrastructure.DTOs.Membership;
using Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationCore.Services
{
    public interface IMembershipService : IBaseService<Membership, MembershipDto>
    {
        public Task<MembershipDto> CreateNewMember(Guid apiKey, MembershipDto dto);
        public Task<MembershipResponse> GetMembershipById(Guid? id);
        public Task<Membership> GetMembershipByIdKey(Guid? id, Guid? apiKey);
        public Task<string> DeleteMembership(Guid id);
        public Task<MembershipDto> UpdateMemberShip(Guid id, UpMembership update,Guid apiKey);
        public Task<Membership> GetMembershipByIdd(Guid? id);
    }
}