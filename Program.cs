using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SomaShare.Data;
using SomaShare.Models;
using SomaShare.Services;

var builder = WebApplication.CreateBuilder(args);

// Add MVC + Razor Pages + Blazor Server
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure()
    ));

// ASP.NET Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Cookie settings — redirect to our Blazor login page
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/account/login";
    options.LogoutPath = "/account/logout";
    options.AccessDeniedPath = "/account/login";
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
});

// Register custom services
builder.Services.AddScoped<TextbookService>();
builder.Services.AddScoped<OfferService>();
builder.Services.AddScoped<WantedAdService>();
builder.Services.AddScoped<ReviewService>();

var app = builder.Build();

// Seed roles, users, and sample data on startup
using (var scope = app.Services.CreateScope())
{
    await DbInitializer.SeedRolesAndUsers(scope.ServiceProvider);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Map Razor Pages FIRST so /LoginHandler is found before the Blazor fallback
app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapBlazorHub();

// Blazor fallback — must be LAST
app.MapFallbackToPage("/_Host");

app.Run();
