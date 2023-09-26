using Infrastructure.DTOs;
using Infrastructure.Models;
using System.Threading.Tasks;
using System;

namespace ApplicationCore.Services
{
    public interface IMemberShipCardService : IBaseService<MembershipCard, MemberShipCardDto>
    {
        public Task<MemberShipCardDto> CreateMemberShipCard(MemberShipCardDto dto);
        public Task<MemberShipCardDto> GetMemberShipCardDetail(Guid id);
        public Task<bool> DeleteMemberShipCard(Guid id);

        public Task<MemberShipCardDto> UpdateMemberShipCard(MemberShipCardDto dto);
    }
}
