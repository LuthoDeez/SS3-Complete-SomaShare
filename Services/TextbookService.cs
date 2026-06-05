using Microsoft.EntityFrameworkCore;
using SomaShare.Data;
using SomaShare.Models;

namespace SomaShare.Services
{
    public class TextbookService
    {
        private readonly ApplicationDbContext _context;

        public TextbookService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Textbook>> GetAllTextbooksAsync()
        {
            return await _context.Textbooks
                .Include(t => t.Seller)
                .OrderByDescending(t => t.DatePosted)
                .ToListAsync();
        }

        public async Task<Textbook?> GetTextbookByIdAsync(int id)
        {
            return await _context.Textbooks
                .Include(t => t.Seller)
                .Include(t => t.Offers)
                .FirstOrDefaultAsync(t => t.TextbookId == id);
        }

        public async Task CreateTextbookAsync(Textbook textbook)
        {
            _context.Textbooks.Add(textbook);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTextbookAsync(Textbook textbook)
        {
            _context.Textbooks.Update(textbook);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTextbookAsync(int id, string requestingUserId)
        {
            var book = await _context.Textbooks.FindAsync(id);
            if (book == null) return;

            if (book.SellerId != requestingUserId)
                throw new UnauthorizedAccessException("You can only delete your own listings.");

            _context.Textbooks.Remove(book);
            await _context.SaveChangesAsync();
        }
    }
}
