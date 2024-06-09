using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketingHub.Models
{
    public class ApplicationUser:IdentityUser
    {
        [Required]
        public string Name {  get; set; }

        public static explicit operator ApplicationUser(Task<IdentityUser?> v)
        {
            throw new NotImplementedException();
        }
    }
}
