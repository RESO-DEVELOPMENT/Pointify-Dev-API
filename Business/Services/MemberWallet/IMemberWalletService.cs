using Infrastructure.DTOs;
using Infrastructure.Models;
using System.Threading.Tasks;
using System;

namespace ApplicationCore.Services
{
    public interface IMemberWalletService : IBaseService<MemberWallet, MemberWalletDto>
    {
        public Task<MemberWalletDto> CreateWallet(MemberWalletDto dto);
        public Task<MemberWalletDto> GetWalletDetail(Guid id);
        public Task<bool> HideWallet(Guid id, string value);

        public Task<MemberWalletDto> UpdateWallet(MemberDto dto);
    }
}
