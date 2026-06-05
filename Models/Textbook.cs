using System.ComponentModel.DataAnnotations;

namespace SomaShare.Models
{
    public class Textbook
    {
        [Key]
        public int TextbookId { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string? Author { get; set; }
        public string? ISBN { get; set; }
        public string? Condition { get; set; }

        [Range(1, 100000)]
        public decimal Price { get; set; }

        public string? Campus { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsAvailable { get; set; } = true;
        public DateTime DatePosted { get; set; } = DateTime.Now;

        [Required]
        public string SellerId { get; set; } = string.Empty;
        public ApplicationUser? Seller { get; set; }

        public ICollection<Offer> Offers { get; set; } = new List<Offer>();
    }
}
