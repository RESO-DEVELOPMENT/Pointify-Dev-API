using Infrastructure.DTOs;
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
        public Task<Membership> GetMembershipById(Guid id);
        public Task<string> DeleteMembership(Guid id);
        public Task<MembershipDto> UpdateMemberShip(Guid id, UpMembership update);
        public Task<Membership> ScanMemberCode(string code);
    }
}