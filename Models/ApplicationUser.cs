using Microsoft.AspNetCore.Identity;

namespace SomaShare.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public string? Campus { get; set; }
        public string? Role { get; set; }
        public int TrustScore { get; set; } = 0;
        public DateTime DateJoined { get; set; } = DateTime.Now;

        // Navigation properties
        public ICollection<Review> ReviewsGiven { get; set; } = new List<Review>();
        public ICollection<Review> ReviewsReceived { get; set; } = new List<Review>();
        public ICollection<WantedAd> WantedAds { get; set; } = new List<WantedAd>();
    }
}
