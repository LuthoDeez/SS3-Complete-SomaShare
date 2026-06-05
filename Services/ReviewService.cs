using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SomaShare.Data;
using SomaShare.Models;

namespace SomaShare.Services
{
    public class ReviewService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReviewService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<List<Review>> GetReviewsForUserAsync(string userId)
        {
            return await _context.Reviews
                .Include(r => r.Reviewer)
                .Where(r => r.RevieweeId == userId)
                .OrderByDescending(r => r.DatePosted)
                .ToListAsync();
        }

        public async Task<List<Review>> GetReviewsByUserAsync(string userId)
        {
            return await _context.Reviews
                .Include(r => r.Reviewee)
                .Where(r => r.ReviewerId == userId)
                .OrderByDescending(r => r.DatePosted)
                .ToListAsync();
        }

        public async Task CreateReviewAsync(Review review)
        {
            bool alreadyReviewed = await _context.Reviews.AnyAsync(r =>
                r.ReviewerId == review.ReviewerId &&
                r.RevieweeId == review.RevieweeId);
            if (alreadyReviewed)
                throw new InvalidOperationException("You have already reviewed this user.");

            review.DatePosted = DateTime.Now;
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            await RecalculateTrustScore(review.RevieweeId);
        }

        public async Task RecalculateTrustScore(string userId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.RevieweeId == userId)
                .ToListAsync();

            double avgRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0;

            int completedTx = await _context.Transactions
                .Include(t => t.Offer)
                .Where(t => t.Offer != null &&
                            t.Offer.BuyerId == userId &&
                            t.IsComplete)
                .CountAsync();

            int score = (int)Math.Round((avgRating / 5.0) * 60) + (completedTx * 5);
            score = Math.Min(score, 100);

            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.TrustScore = score;
                await _userManager.UpdateAsync(user);
            }
        }

        public async Task DeleteReviewAsync(int reviewId, string requestingUserId)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review == null) return;
            if (review.ReviewerId != requestingUserId)
                throw new UnauthorizedAccessException("You can only delete your own reviews.");
            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
        }
    }
}
