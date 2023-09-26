using Infrastructure.DTOs;
using Infrastructure.DTOs.Account;
using Infrastructure.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationCore.Services
{
    public interface ILoginService
    {
        public Task<LoginResponse> Login(UserModel user);

    }
}
