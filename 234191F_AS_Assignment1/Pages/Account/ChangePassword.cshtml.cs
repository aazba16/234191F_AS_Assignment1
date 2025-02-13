using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using _234191F_AS_Assignment1.ViewModels;
using _234191F_AS_Assignment1.Pages.Services; // Import AccountService
using System.Threading.Tasks;
using _234191F_AS_Assignment1.Model;
using _234191F_AS_Assignment1.Pages.Services;
using Microsoft.EntityFrameworkCore;

namespace _234191F_AS_Assignment1.Pages.Account
{
    public class ChangePasswordPageModel : PageModel
    {
        private readonly UserManager<Register> _userManager;
        private readonly SignInManager<Register> _signInManager;
        private readonly AccountServices _accountService; // Inject AccountService
        private readonly AuthDbContext _context;

        public ChangePasswordPageModel(UserManager<Register> userManager, SignInManager<Register> signInManager, AccountServices accountService, AuthDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _accountService = accountService; // Assign injected service
            _context = context;
        }

        [BindProperty]
        public ChangePasswordModel ChangePassword { get; set; }  // Bind to the ViewModel

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            // Get the most recent password change date from PasswordHistories
            var lastPasswordChange = await _context.PasswordHistories
                .Where(p => p.UserId == user.Id)
                .OrderByDescending(p => p.DateChanged) // Get the most recent change
                .FirstOrDefaultAsync();

            if (lastPasswordChange != null)
            {
                // Check if the password can be changed based on the minimum password age
                var minimumPasswordAge = TimeSpan.FromMinutes(30);  // Example: Minimum password age of 30 minutes

                if (lastPasswordChange.DateChanged.Add(minimumPasswordAge) > DateTime.UtcNow)
                {
                    ModelState.AddModelError(string.Empty, $"You must wait at least {minimumPasswordAge.TotalMinutes} minutes before changing your password again.");
                    return Page();
                }
            }

            // Check if the entered current password is correct
            var result = await _userManager.CheckPasswordAsync(user, ChangePassword.CurrentPassword);

            if (!result)
            {
                ModelState.AddModelError(string.Empty, "Current password is incorrect.");
                return Page();
            }

            // Now, check if the new password is reused from the last 2 passwords
            if (!await _accountService.CheckPasswordHistory(user.Id, ChangePassword.NewPassword))
            {
                ModelState.AddModelError(string.Empty, "You cannot reuse your last 2 passwords.");
                return Page();
            }

            // Now change the password since all checks passed
            var changePasswordResult = await _userManager.ChangePasswordAsync(user, ChangePassword.CurrentPassword, ChangePassword.NewPassword);

            if (changePasswordResult.Succeeded)
            {
                // Add the new password to history after a successful password change
                await _accountService.AddPasswordToHistory(user.Id, ChangePassword.NewPassword);

                // Optionally, log out the user and prompt them to log in again
                await _signInManager.SignOutAsync();
                return RedirectToPage("/Account/PasswordChangedSuccessfully");
            }

            // If there are errors
            foreach (var error in changePasswordResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }



    }
}
