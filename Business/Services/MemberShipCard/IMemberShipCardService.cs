using Infrastructure.DTOs;
using Infrastructure.Models;
using System.Threading.Tasks;
using System;

namespace ApplicationCore.Services
{
    public interface IMemberShipCardService : IBaseService<MembershipCard, MemberShipCardDto>
    {
        public Task<MemberShipCardDto> CreateMemberShipCard(MemberShipCardDto dto);
        public Task<MemberShipCardDto> GetMemberShipCardDetail(Guid id, Guid apiKey);
        public Task<bool> DeleteMemberShipCard(Guid id);

        public Task<MemberShipCardDto> UpdateMemberShipCard(Guid id, MemberShipCardDto dto, Guid apiKey);
        public Task<bool> AddCodeForMember(Guid id, Guid apiKey);
    }
}
