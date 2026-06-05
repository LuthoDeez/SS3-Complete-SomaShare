using System.ComponentModel.DataAnnotations;

namespace SomaShare.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }

        public DateTime TransactionDate { get; set; } = DateTime.Now;

        public string PaymentMethod { get; set; } = "Cash on Meetup";

        public bool IsComplete { get; set; } = false;

        public int OfferId { get; set; }
        public Offer? Offer { get; set; }
    }
}
