
using Infrastructure.DTOs;
using Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApplicationCore.Services
{
    public interface IAccountService : IBaseService<Account, AccountDto>
    {
        Task<Account> GetByUsernameAsync(string username);
        Task<bool> DeleteUsernameAsync(string username);
        Task<bool> HideUsernameAsync(string username, string value);
    }
}
