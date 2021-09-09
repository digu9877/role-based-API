using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication6.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Column(TypeName = "character varying(150)")]
        public string FullName { get; set; }
    }
}
