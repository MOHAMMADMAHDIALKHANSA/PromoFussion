
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarketingHub.Models
{
    public class Feedback
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Customer ID is required")]
        public string CustomerId { get; set; }

        [Required(ErrorMessage = "Marketing Agency ID is required")]
        public int MarketingAgencyId { get; set; }

        [Required(ErrorMessage = "Rating is required")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }

        public string? Comment { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        public Customer Customer { get; set; }
        public MarketingAgency MarketingAgency { get; set; }
    }
}
