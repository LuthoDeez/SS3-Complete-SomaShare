using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SomaShare.Models;

namespace SomaShare.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Textbook> Textbooks { get; set; }
        public DbSet<Offer> Offers { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<WantedAd> Adverts { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ── Textbook ──────────────────────────────────────────────
            builder.Entity<Textbook>(entity =>
            {
                entity.HasKey(t => t.TextbookId);

                entity.Property(t => t.Price)
                      .HasColumnType("decimal(18,2)");

                entity.HasOne(t => t.Seller)
                      .WithMany()
                      .HasForeignKey(t => t.SellerId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Index on searchable fields for performance
                entity.HasIndex(t => t.Title);
                entity.HasIndex(t => t.ISBN);
                entity.HasIndex(t => t.SellerId);
            });

            // ── Offer ─────────────────────────────────────────────────
            builder.Entity<Offer>(entity =>
            {
                entity.HasKey(o => o.OfferId);

                entity.Property(o => o.OfferAmount)
                      .HasColumnType("decimal(18,2)");

                entity.HasOne(o => o.Textbook)
                      .WithMany(t => t.Offers)
                      .HasForeignKey(o => o.TextbookId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(o => o.Buyer)
                      .WithMany()
                      .HasForeignKey(o => o.BuyerId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(o => o.TextbookId);
                entity.HasIndex(o => o.BuyerId);
            });

            // ── Transaction ───────────────────────────────────────────
            builder.Entity<Transaction>(entity =>
            {
                entity.HasKey(t => t.TransactionId);

                entity.HasOne(t => t.Offer)
                      .WithOne()
                      .HasForeignKey<Transaction>(t => t.OfferId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ── Review ────────────────────────────────────────────────
            // Many-to-Many style: one user can review many users
            builder.Entity<Review>(entity =>
            {
                entity.HasKey(r => r.ReviewId);

                entity.HasOne(r => r.Reviewer)
                      .WithMany(u => u.ReviewsGiven)
                      .HasForeignKey(r => r.ReviewerId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.Reviewee)
                      .WithMany(u => u.ReviewsReceived)
                      .HasForeignKey(r => r.RevieweeId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Unique index: one review per reviewer-reviewee pair
                entity.HasIndex(r => new { r.ReviewerId, r.RevieweeId })
                      .IsUnique();
            });

            // ── WantedAd ──────────────────────────────────────────────
            builder.Entity<WantedAd>(entity =>
            {
                entity.ToTable("Adverts");
                entity.HasKey(w => w.Id);

                entity.Property(w => w.MaxPrice)
                      .HasColumnType("decimal(18,2)");

                entity.HasOne(w => w.User)
                      .WithMany(u => u.WantedAds)
                      .HasForeignKey(w => w.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(w => w.UserId);
                entity.HasIndex(w => w.CourseCode);
            });
        }
    }
}
