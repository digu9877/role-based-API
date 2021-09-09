using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace WebApplication6.Models
{
    public class LoginModel
    {
        [Required]
        public String Email { get; set; }
        [Required]
        public String Password { get; set; }

    }
}
