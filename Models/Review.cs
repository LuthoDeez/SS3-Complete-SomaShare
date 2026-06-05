using System.ComponentModel.DataAnnotations;

namespace SomaShare.Models
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        public string Comment { get; set; } = string.Empty;

        public DateTime DatePosted { get; set; } = DateTime.Now;

        [Required]
        public string ReviewerId { get; set; } = string.Empty;
        public ApplicationUser? Reviewer { get; set; }

        [Required]
        public string RevieweeId { get; set; } = string.Empty;
        public ApplicationUser? Reviewee { get; set; }
    }
}
