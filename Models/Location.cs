using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarketingHub.Models
{
    public class Location
    {
        [Key]
        public int LocationId { get; set; } = 0;

        [Required(ErrorMessage = "City is required")]
        public string City { get; set; } = string.Empty;

        [Url(ErrorMessage = "Invalid URL format")]
        public string ImageUrl { get; set; } = "";

        [Required(ErrorMessage = "Postal code is required")]
        public int PostalCode { get; set; }

        [Required(ErrorMessage = "Building name is required")]
        public string BuildingName { get; set; } = "";

        [Required(ErrorMessage = "Region is required")]
        public string Region { get; set; } = "";

        [Required(ErrorMessage = "URL route is required")]
        public string UrlRoute { get; set; } = "";
        ICollection<MarketingAgency> Agency { get; set; }
        
    }
}
