using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SomaShare.Models;

namespace SomaShare.Data
{
    public static class DbInitializer
    {
        public static async Task SeedRolesAndUsers(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var db = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // ── Seed Roles ────────────────────────────────────────────
            string[] roles = { "Admin", "Seller", "Buyer" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // ── Seed Admin ────────────────────────────────────────────
            ApplicationUser? admin = await userManager.FindByEmailAsync("admin@somashare.ac.za");
            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = "admin@somashare.ac.za",
                    Email = "admin@somashare.ac.za",
                    FullName = "Admin User",
                    EmailConfirmed = true,
                    DateJoined = DateTime.Now,
                    TrustScore = 100
                };
                await userManager.CreateAsync(admin, "Admin@1234");
                await userManager.AddToRoleAsync(admin, "Admin");
            }

            // ── Seed Sample Seller ─────────────────────────────────────
            ApplicationUser? seller = await userManager.FindByEmailAsync("seller@wits.ac.za");
            if (seller == null)
            {
                seller = new ApplicationUser
                {
                    UserName = "seller@wits.ac.za",
                    Email = "seller@wits.ac.za",
                    FullName = "Thabo Nkosi",
                    Campus = "Wits",
                    EmailConfirmed = true,
                    DateJoined = DateTime.Now.AddMonths(-3),
                    TrustScore = 45
                };
                await userManager.CreateAsync(seller, "Seller@1234");
                await userManager.AddToRoleAsync(seller, "Seller");
            }

            // ── Seed Sample Buyer ──────────────────────────────────────
            ApplicationUser? buyer = await userManager.FindByEmailAsync("buyer@uct.ac.za");
            if (buyer == null)
            {
                buyer = new ApplicationUser
                {
                    UserName = "buyer@uct.ac.za",
                    Email = "buyer@uct.ac.za",
                    FullName = "Amahle Dlamini",
                    Campus = "UCT",
                    EmailConfirmed = true,
                    DateJoined = DateTime.Now.AddMonths(-1),
                    TrustScore = 10
                };
                await userManager.CreateAsync(buyer, "Buyer@1234");
                await userManager.AddToRoleAsync(buyer, "Buyer");
            }

            // ── Refresh to get IDs ─────────────────────────────────────
            seller = await userManager.FindByEmailAsync("seller@wits.ac.za");
            buyer = await userManager.FindByEmailAsync("buyer@uct.ac.za");

            // ── Seed Textbooks ─────────────────────────────────────────
            if (!await db.Textbooks.AnyAsync())
            {
                var books = new List<Textbook>
                {
                    new Textbook
                    {
                        Title = "Introduction to Algorithms",
                        Author = "Cormen, Leiserson, Rivest",
                        ISBN = "9780262033848",
                        Condition = "Good",
                        Price = 350,
                        Campus = "Wits",
                        IsAvailable = true,
                        DatePosted = DateTime.Now.AddDays(-10),
                        SellerId = seller!.Id
                    },
                    new Textbook
                    {
                        Title = "Calculus: Early Transcendentals",
                        Author = "James Stewart",
                        ISBN = "9781285741550",
                        Condition = "Fair",
                        Price = 280,
                        Campus = "Wits",
                        IsAvailable = true,
                        DatePosted = DateTime.Now.AddDays(-7),
                        SellerId = seller!.Id
                    },
                    new Textbook
                    {
                        Title = "Business Management in Africa",
                        Author = "N. Mashaba",
                        ISBN = "9780796236401",
                        Condition = "New",
                        Price = 420,
                        Campus = "UCT",
                        IsAvailable = true,
                        DatePosted = DateTime.Now.AddDays(-5),
                        SellerId = seller!.Id
                    },
                    new Textbook
                    {
                        Title = "Data Structures and Algorithms",
                        Author = "Mark Weiss",
                        ISBN = "9780132576277",
                        Condition = "Good",
                        Price = 300,
                        Campus = "UNISA",
                        IsAvailable = true,
                        DatePosted = DateTime.Now.AddDays(-3),
                        SellerId = seller!.Id
                    },
                    new Textbook
                    {
                        Title = "Financial Accounting",
                        Author = "Kimmel, Weygandt",
                        ISBN = "9780470534793",
                        Condition = "Poor",
                        Price = 150,
                        Campus = "Wits",
                        IsAvailable = true,
                        DatePosted = DateTime.Now.AddDays(-2),
                        SellerId = seller!.Id
                    }
                };

                db.Textbooks.AddRange(books);
                await db.SaveChangesAsync();

                // ── Seed a Sample Offer ────────────────────────────────
                var firstBook = await db.Textbooks.FirstAsync();
                var offer = new Offer
                {
                    TextbookId = firstBook.TextbookId,
                    BuyerId = buyer!.Id,
                    OfferAmount = 300,
                    Status = "Pending",
                    DateMade = DateTime.Now.AddDays(-1)
                };
                db.Offers.Add(offer);
                await db.SaveChangesAsync();

                // ── Seed a Wanted Ad ───────────────────────────────────
                var wantedAd = new WantedAd
                {
                    Title = "Looking for Statistics 101 textbook",
                    CourseCode = "STAT101",
                    MaxPrice = 200,
                    DatePosted = DateTime.Now.AddDays(-2),
                    UserId = buyer!.Id
                };
                db.Adverts.Add(wantedAd);

                // ── Seed a Review ──────────────────────────────────────
                var review = new Review
                {
                    ReviewerId = buyer!.Id,
                    RevieweeId = seller!.Id,
                    Rating = 5,
                    Comment = "Great seller! Book was in perfect condition and meetup was smooth.",
                    DatePosted = DateTime.Now.AddDays(-1)
                };
                db.Reviews.Add(review);

                // Update seller trust score
                seller.TrustScore += 10;
                await userManager.UpdateAsync(seller);

                await db.SaveChangesAsync();
            }
        }
    }
}
