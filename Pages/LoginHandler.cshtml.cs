using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SomaShare.Models;

namespace SomaShare.Pages
{
    [IgnoreAntiforgeryToken]
    public class LoginHandlerModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public LoginHandlerModel(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        // Parameters MUST match form field names exactly (case-insensitive in MVC but be explicit)
        public async Task<IActionResult> OnPostAsync(string Email, string Password)
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
                return Redirect("/account/login?error=fail");

            var result = await _signInManager.PasswordSignInAsync(
                Email,
                Password,
                isPersistent: false,
                lockoutOnFailure: false);

            if (result.Succeeded)
                return Redirect("/dashboard");

            return Redirect("/account/login?error=fail");
        }
    }
}
