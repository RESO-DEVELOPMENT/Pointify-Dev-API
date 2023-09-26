using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.DTOs
{
    public class AccountDto : BaseDto
    {
        public string Username { get; set; }
        public int? RoleId { get; set; }
        public string FirstName { get; set; }
        public string Password { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string ImgUrl { get; set; }
        public bool IsActive { get; set; }
        public virtual BrandDto Brand { get; set; }
    }
}
