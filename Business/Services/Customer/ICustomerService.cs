using Infrastructure.DTOs;
using Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ApplicationCore.Services
{
    public interface ICustomerService : IBaseService<Customer, CustomerDto>
    {
        Task<Customer> GetByUsernameAsync(string username);
        public Task<Customer> GetByIdAsync(Guid id);
        Task<bool> DeleteUsernameAsync(string username);
        Task<bool> HideUsernameAsync(string username, string value);
    }
}
