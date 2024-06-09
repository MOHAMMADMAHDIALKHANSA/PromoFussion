using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarketingHub.Models
{
    public class Appointment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AppointmentId { get; set; }

        public string CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }

        public int MarketingAgencyId { get; set; }
        [ForeignKey("MarketingAgencyId")]
        public MarketingAgency MarketingAgency { get; set; }

        public DateTime RequestedDate { get; set; }
        public string Status { get; set; } // Pending, Accepted, Rejected
    }
}
