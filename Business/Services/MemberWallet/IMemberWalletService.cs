using Infrastructure.DTOs;
using Infrastructure.Models;
using System.Threading.Tasks;
using System;

namespace ApplicationCore.Services
{
    public interface IMemberWalletService : IBaseService<MemberWallet, MemberWalletDto>
    {
        public Task<MemberWalletDto> CreateWallet(MemberWalletDto dto);
        public Task<MemberWallet> GetMemberWalletById(Guid id);
        public Task<bool> HideWallet(Guid id, string value);

        public Task<MemberWalletDto> UpdateWallet(Guid id, UpMemberWallet dto);
        public Task<string> DeleteWallet(Guid id);
    }
}
