using System;
using System.Collections.Generic;

namespace Infrastructure.Models
{
    public partial class Account
    {
        public string Username { get; set; }
        public int RoleId { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public bool DelFlg { get; set; }
        public DateTime InsDate { get; set; }
        public DateTime UpdDate { get; set; }
        public string ImgUrl { get; set; }
        public bool IsActive { get; set; }
        public Guid? BrandId { get; set; }

        public virtual Brand Brand { get; set; }
        public virtual Role Role { get; set; }
    }
}
