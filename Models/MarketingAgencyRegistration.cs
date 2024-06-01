using MarketingHub.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace MarketingHub.Models
{
    public class MarketingAgencyRegistration
    {
        [Key]
        public int MarketingAgencyRegistrationId { get; set; }

        [Required(ErrorMessage = "Registration date is required")]
        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "CustomerId is required")]
        public string CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }

        [Required(ErrorMessage = "MarketingAgencyId is required")]
        public int MarketingAgencyId { get; set; }

        [ForeignKey("MarketingAgencyId")]
        public MarketingAgency MarketingAgency { get; set; }
    }
}