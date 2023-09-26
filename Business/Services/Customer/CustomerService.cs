using ApplicationCore.Utils;
using AutoMapper;
using Infrastructure.DTOs;
using Infrastructure.Helper;
using Infrastructure.Models;
using Infrastructure.Repository;
using Infrastructure.UnitOfWork;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ApplicationCore.Services
{
    public class CustomerService : BaseService<Customer, CustomerDto>, ICustomerService
    {

        public CustomerService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        protected override IGenericRepository<Customer> _repository => _unitOfWork.CustomerRepository;

        public async Task<bool> DeleteUsernameAsync(string username)
        {
            _repository.DeleteUsername(username);
            return await _unitOfWork.SaveAsync() > 0;
        }

        public async Task<Customer> GetByUsernameAsync(string username)
        {
            try
            {
                var result = await _repository.GetFirst(
                    el => el.UserName.Equals(username)
                          && (bool) el.Active,
                    includeProperties: "Brand"
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

        public async Task<Customer> GetByIdAsync(Guid id)
        {
            try
            {
                var result = await _repository.GetFirst(filter: el => el.Id == id 
                                                                && !(bool)el.Active,
                                                                includeProperties: "Brand");
                return result;
            } catch (ErrorObj e)
            {
                throw e;
            }
        }
    }
}