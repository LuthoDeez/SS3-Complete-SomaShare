using System.ComponentModel.DataAnnotations;

namespace SomaShare.Models
{
    public class Offer
    {
        [Key]
        public int OfferId { get; set; }

        [Required]
        [Range(1, 100000)]
        public decimal OfferAmount { get; set; }

        public string Status { get; set; } = "Pending"; // Pending | Accepted | Rejected

        public DateTime DateMade { get; set; } = DateTime.Now;

        public int TextbookId { get; set; }
        public Textbook? Textbook { get; set; }

        [Required]
        public string BuyerId { get; set; } = string.Empty;
        public ApplicationUser? Buyer { get; set; }
    }
}
