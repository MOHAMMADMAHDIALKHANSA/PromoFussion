using MarketingHub.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace MarketingHub.Models
{
    public class MarketingAgency 
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MarketingAgencyId { get; set; }

        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; } = "";

        [Required(ErrorMessage = "Salary is required")]
        public string Salary { get; set; } = "";

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string PhoneNumber { get; set; } = "";

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; } = "";

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }

        [Url(ErrorMessage = "Invalid URL format")]
        public string ImageUrl { get; set; } = "";

        public string? Instagram { get; set; }
        public string? Facebook { get; set; }
        public string? Twitter { get; set; }
        public string? LinkedIn { get; set; }



        public ICollection<Customer> Customers { get; set; } = new List<Customer>();
        public ICollection<MarketingAgencyRegistration> MarketingAgencyRegistrations { get; set; } = new List<MarketingAgencyRegistration>();

        public ICollection<Post>? Post { get; set; }
        [Required(ErrorMessage = "Location is required")]
        public int LocationId { get; set; }

        [ForeignKey("LocationId")]
        public Location? Location { get; set; }
        public string UserId { get; set; } 

        [ForeignKey("UserId")]
        public ApplicationUser applicationUser { get; set; }
    }
}