// This file is kept only if your migrations reference it.
// The active model is WantedAd.cs — do NOT use this class in new code.
namespace SomaShare.Models
{
    public class Advert
    {
        public int AdvertId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? CourseCode { get; set; }
        public DateTime DatePosted { get; set; } = DateTime.Now;
        public string PostedById { get; set; } = string.Empty;
        public ApplicationUser? PostedBy { get; set; }
    }
}
