using Microsoft.EntityFrameworkCore;
using SomaShare.Data;
using SomaShare.Models;

namespace SomaShare.Services
{
    public class WantedAdService
    {
        private readonly ApplicationDbContext _context;

        public WantedAdService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<WantedAd>> GetAllWantedAdsAsync()
        {
            return await _context.Adverts
                .Include(w => w.User)
                .OrderByDescending(w => w.DatePosted)
                .ToListAsync();
        }

        public async Task<WantedAd?> GetWantedAdByIdAsync(int id)
        {
            return await _context.Adverts
                .Include(w => w.User)
                .FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task CreateWantedAdAsync(WantedAd wantedAd)
        {
            wantedAd.DatePosted = DateTime.Now;
            _context.Adverts.Add(wantedAd);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateWantedAdAsync(WantedAd wantedAd)
        {
            _context.Adverts.Update(wantedAd);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteWantedAdAsync(int id, string requestingUserId)
        {
            var ad = await _context.Adverts.FindAsync(id);
            if (ad == null) return;

            if (ad.UserId != requestingUserId)
                throw new UnauthorizedAccessException("You can only delete your own wanted ads.");

            _context.Adverts.Remove(ad);
            await _context.SaveChangesAsync();
        }
    }
}
