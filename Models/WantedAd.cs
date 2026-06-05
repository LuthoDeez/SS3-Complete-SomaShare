using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SomaShare.Models
{
    [Table("Adverts")]
    public class WantedAd
    {
        [Key]
        [Column("AdvertId")]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string? CourseCode { get; set; }

        [Required]
        [Range(1, 100000)]
        public decimal MaxPrice { get; set; }

        public DateTime DatePosted { get; set; } = DateTime.Now;

        [Column("PostedById")]
        public string UserId { get; set; } = string.Empty;

        public ApplicationUser? User { get; set; }
    }
}
