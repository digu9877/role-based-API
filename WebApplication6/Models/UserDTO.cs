using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication6.Models
{
    public class UserDTO
    {
        public UserDTO(string fullName, string email, string userName, string role)
        {
            FullName = fullName;
            Email = email;
            UserName = userName;
            Roles = role;
        }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Token { get; set; }
        public string Roles
        {
            get; set;

        }
    }
}
