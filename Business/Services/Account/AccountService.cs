using ApplicationCore.Utils;
using AutoMapper;
using Infrastructure.DTOs;
using Infrastructure.Helper;
using Infrastructure.Models;
using Infrastructure.Repository;
using Infrastructure.UnitOfWork;
using System;
using System.Threading.Tasks;

namespace ApplicationCore.Services
{
    public class AccountService : BaseService<Account, AccountDto>, IAccountService
    {
        public AccountService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        protected override IGenericRepository<Account> _repository => _unitOfWork.AccountRepository;

        public async Task<bool> DeleteUsernameAsync(string username)
        {
            _repository.DeleteUsername(username);
            return await _unitOfWork.SaveAsync() > 0;
        }

        public async Task<Account> GetByUsernameAsync(string username)
        {
            try
            {
                var result = await _repository.GetFirst(
                    el => el.Username.Equals(username)
                          && !el.DelFlg
                          && el.IsActive,
                    includeProperties: "Brand,Role"
                );
                return result;
            }
            catch (ErrorObj e)
            {
                throw e;
            }
        }

        public async Task<bool> HideUsernameAsync(string username, string value)
        {
            _repository.HideUsername(username, value);
            return await _unitOfWork.SaveAsync() > 0;
        }

        /* public async Task<bool> AddAccount(AccountDto dto)
         {
             DateTime now = Common.GetCurrentDatetime();
             dto.InsDate = now;
             dto.UpdDate = now;
             dto.RoleId = AppConstant.EnvVar.BrandId;
             dto.Password = Common.EncodeToBase64(dto.Password);
             dto.IsActive = false;
             var account = _mapper.Map<Account>(dto);
             if (dto.HasBrand)
             {
                 IGenericRepository<Brand> brandRepo = _unitOfWork.BrandRepository;
 
                 var brand = await brandRepo.GetFirst(filter: el => el.BrandCode == dto.Brand.BrandCode);
                 brand.InsDate = now;
                 brand.UpdDate = now;
                 if (brand != null)
                 {
                     account.Brand = brand;
                     _repository.Add(account);
                 }
             }
             return await _unitOfWork.SaveAsync() > 0;
         }*/
    }
}