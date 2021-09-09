using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication6.Models
{
    public class ApplicationUserModel
    {
       
        public string Email { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }

        public string Role { get; set; }
    }
}
