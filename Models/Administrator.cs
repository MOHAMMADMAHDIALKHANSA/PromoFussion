using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarketingHub.Models
{
   

    
        public class Administrator
        {
            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AdministratorId { get; set; }

            [Required(ErrorMessage = "First name is required")]
            public string FirstName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Last name is required")]
            public string LastName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Role is required")]
            public string Role { get; set; } = string.Empty;

            [Required(ErrorMessage = "Email is required")]
            [EmailAddress(ErrorMessage = "Invalid email address")]
            public string Email { get; set; } = string.Empty;

            [Url(ErrorMessage = "Invalid image URL")]
            public string ImgUrl { get; set; } = string.Empty;
        }
}
