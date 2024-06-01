using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarketingHub.Models
{
    public class Review
    {
       

        [Required(ErrorMessage = "Please enter your name.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Please provide a rating.")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }

        [Required(ErrorMessage = "Please enter your review.")]
        [MaxLength(500, ErrorMessage = "Review must be 500 characters or less.")]
        public string UserReview { get; set; }

        // Foreign key for Customer
        public string CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public int Id { get; set; }
    }
}
