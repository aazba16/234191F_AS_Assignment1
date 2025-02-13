using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using _234191F_AS_Assignment1.ViewModels;
using System.Threading.Tasks;
using _234191F_AS_Assignment1.Model;

namespace _234191F_AS_Assignment1.Pages.Account
{
    public class ResetPasswordPageModel : PageModel // Rename class to avoid conflict with ViewModel
    {
        private readonly UserManager<Register> _userManager;

        // No need for a constructor; ASP.NET Core handles the injection automatically
        public ResetPasswordPageModel(UserManager<Register> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public ResetPasswordModel Input { get; set; }  // Bind to the ViewModel

        public async Task<IActionResult> OnGetAsync(string token, string email)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
            {
                return RedirectToPage("/Account/Error");
            }

            Input = new ResetPasswordModel { Token = token, Email = email };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                return NotFound();
            }

            var resetPasswordResult = await _userManager.ResetPasswordAsync(user, Input.Token, Input.NewPassword);
            if (resetPasswordResult.Succeeded)
            {
                // Redirect to the Password Reset Success page
                return RedirectToPage("/Account/PasswordResetSuccess");
            }

            foreach (var error in resetPasswordResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }


    }
}
