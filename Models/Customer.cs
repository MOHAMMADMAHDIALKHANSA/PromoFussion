using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace MarketingHub.Models
{
    public class Customer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string CustomerId { get; set; }

        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; } = "";

        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; } = "";

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = "";

        [Phone(ErrorMessage = "Invalid phone number")]
        public string PhoneNumber { get; set; } = "";

        public int? MarketingAgencyId { get; set; }

        [ForeignKey("MarketingAgencyId")]
        public MarketingAgency MarketingAgency { get; set; }

        public ICollection<MarketingAgencyRegistration> MarketingAgencyRegistrations { get; set; } = new List<MarketingAgencyRegistration>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();


    }
}