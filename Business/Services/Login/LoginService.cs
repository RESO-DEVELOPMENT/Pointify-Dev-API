using ApplicationCore.Utils;
using Infrastructure.DTOs;
using Infrastructure.DTOs.Account;
using Infrastructure.Helper;
using Infrastructure.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Services
{
    public class LoginService : ILoginService
    {
        private readonly IAccountService _accountService;
        private IConfiguration _config;


        public LoginService(IAccountService accountService, IConfiguration configuration)
        {
            _accountService = accountService;
            _config = configuration;
        }

        public async Task<LoginResponse> Login(UserModel userInfo)
        {
            try
            {
                LoginResponse result = null;
                var account = await _accountService.GetByUsernameAsync(userInfo.Username);
                if (account != null)
                {
                    result = new LoginResponse();
                    string message;
                    int status;
                    if (account.Role.RoleId != AppConstant.EnvVar.AdminId)
                    {
                        account.Password = Common.DecodeFromBase64(account.Password);
                    }
                    if (account.Password == userInfo.Password)
                    {
                        UserInfo user = null;
                        var token = GenerateJSONWebToken(account);
                        switch (account.Role.RoleId)
                        {
                            case AppConstant.EnvVar.AdminId:
                                user = new UserInfo
                                {
                                    Username = account.Username,
                                    Token = token,
                                    RoleName = account.Role.Name,
                                    FullName = account.FirstName + " " + account.LastName
                                };
                                break;
                            case AppConstant.EnvVar.BrandId:
                                user = new UserInfo
                                {
                                    Username = account.Username,
                                    BrandCode = account.Brand.BrandCode,
                                    BrandId = account.Brand.BrandId,
                                    Token = token,
                                    RoleName = account.Role.Name,
                                    FullName = account.FirstName + " " + account.LastName

                                };
                                break;
                        }
                        status = (int)HttpStatusCode.OK;
                        message = AppConstant.ErrMessage.Login_Success;
                        result.Data = user;
                    }
                    else
                    {
                        status = (int)HttpStatusCode.Unauthorized;
                        message = AppConstant.ErrMessage.Login_Fail;
                    }
                    result.Message = message;
                    result.Status = status;
                }
                return result;
            }
            catch (ErrorObj e)
            {
                throw e;
            }

        }
        private string GenerateJSONWebToken(Account account)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_config["AppSettings:SecretKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var claims = new[]
            {
                    new Claim(ClaimTypes.Name, account.Username),
                    new Claim(ClaimTypes.Role, account.Role.Name)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = credentials
            };
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(securityToken);
            /*var token = new JwtSecurityToken(_config["AppSettings:Issuer"],
              _config["AppSettings:Issuer"],
              claims,
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);*/

            /*return new JwtSecurityTokenHandler().WriteToken(token);*/
            return token;
        }

    }
}
