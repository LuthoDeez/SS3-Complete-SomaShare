using Microsoft.EntityFrameworkCore;
using SomaShare.Data;
using SomaShare.Models;

namespace SomaShare.Services
{
    public class OfferService
    {
        private readonly ApplicationDbContext _context;

        public OfferService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Offer>> GetOffersForTextbookAsync(int textbookId)
        {
            return await _context.Offers
                .Include(o => o.Buyer)
                .Where(o => o.TextbookId == textbookId)
                .OrderByDescending(o => o.DateMade)
                .ToListAsync();
        }

        public async Task CreateOfferAsync(Offer offer)
        {
            var textbook = await _context.Textbooks.FindAsync(offer.TextbookId);
            if (textbook == null)
                throw new InvalidOperationException("Textbook not found.");
            if (!textbook.IsAvailable)
                throw new InvalidOperationException("This textbook is no longer available.");
            if (textbook.SellerId == offer.BuyerId)
                throw new InvalidOperationException("You cannot make an offer on your own listing.");

            bool alreadyOffered = await _context.Offers.AnyAsync(o =>
                o.TextbookId == offer.TextbookId &&
                o.BuyerId == offer.BuyerId &&
                o.Status == "Pending");
            if (alreadyOffered)
                throw new InvalidOperationException("You already have a pending offer on this listing.");

            offer.Status = "Pending";
            offer.DateMade = DateTime.Now;
            _context.Offers.Add(offer);
            await _context.SaveChangesAsync();
        }

        public async Task AcceptOfferAsync(int offerId, string sellerUserId)
        {
            var offer = await _context.Offers
                .Include(o => o.Textbook)
                .FirstOrDefaultAsync(o => o.OfferId == offerId);

            if (offer == null)
                throw new InvalidOperationException("Offer not found.");
            if (offer.Textbook == null)
                throw new InvalidOperationException("Textbook not found.");
            if (offer.Textbook.SellerId != sellerUserId)
                throw new UnauthorizedAccessException("You can only accept offers on your own listings.");

            bool alreadyAccepted = await _context.Offers
                .AnyAsync(o => o.TextbookId == offer.TextbookId && o.Status == "Accepted");
            if (alreadyAccepted)
                throw new InvalidOperationException("An offer has already been accepted for this textbook.");

            offer.Status = "Accepted";
            offer.Textbook.IsAvailable = false;

            var otherOffers = await _context.Offers
                .Where(o => o.TextbookId == offer.TextbookId &&
                            o.OfferId != offerId &&
                            o.Status == "Pending")
                .ToListAsync();
            foreach (var other in otherOffers)
                other.Status = "Rejected";

            var transaction = new Transaction
            {
                OfferId = offer.OfferId,
                TransactionDate = DateTime.Now,
                PaymentMethod = "Cash on Meetup",
                IsComplete = false
            };
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task RejectOfferAsync(int offerId, string sellerUserId)
        {
            var offer = await _context.Offers
                .Include(o => o.Textbook)
                .FirstOrDefaultAsync(o => o.OfferId == offerId);
            if (offer == null) return;
            if (offer.Textbook == null) return;
            if (offer.Textbook.SellerId != sellerUserId)
                throw new UnauthorizedAccessException("You can only reject offers on your own listings.");

            offer.Status = "Rejected";
            await _context.SaveChangesAsync();
        }

        public async Task MarkTransactionCompleteAsync(int transactionId, string buyerUserId)
        {
            var transaction = await _context.Transactions
                .Include(t => t.Offer)
                    .ThenInclude(o => o!.Textbook)
                .FirstOrDefaultAsync(t => t.TransactionId == transactionId);

            if (transaction == null)
                throw new InvalidOperationException("Transaction not found.");
            if (transaction.Offer == null)
                throw new InvalidOperationException("Associated offer not found.");
            if (transaction.Offer.BuyerId != buyerUserId)
                throw new UnauthorizedAccessException("Only the buyer can mark a transaction complete.");
            if (transaction.IsComplete)
                throw new InvalidOperationException("Transaction is already marked complete.");

            transaction.IsComplete = true;
            await _context.SaveChangesAsync();

            // Award +5 trust score to seller
            if (transaction.Offer.Textbook != null)
            {
                var seller = await _context.Users
                    .OfType<ApplicationUser>()
                    .FirstOrDefaultAsync(u => u.Id == transaction.Offer.Textbook.SellerId);
                if (seller != null)
                {
                    seller.TrustScore = Math.Min(seller.TrustScore + 5, 100);
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}
