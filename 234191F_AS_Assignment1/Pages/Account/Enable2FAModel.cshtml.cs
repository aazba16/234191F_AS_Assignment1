using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using _234191F_AS_Assignment1.Model;
using _234191F_AS_Assignment1.ViewModels;
using _234191F_AS_Assignment1.Pages.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace _234191F_AS_Assignment1.Pages.Account
{
    public class Enable2FAModel : PageModel
    {
        private readonly UserManager<Register> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly TwoFactorAuthService _twoFactorAuthService;

        public string EmailMessage { get; set; }

        public Enable2FAModel(UserManager<Register> userManager, IEmailSender emailSender, TwoFactorAuthService twoFactorAuthService)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _twoFactorAuthService = twoFactorAuthService;
        }

        [BindProperty]
        public string TwoFactorCode { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // Step 1: Generate the 2FA secret key for this user
            var secretKey = _twoFactorAuthService.GenerateSecretKey();

            // Step 2: Store the secret key in the user's record
            user.GoogleAuthenticatorSecret = secretKey;
            await _userManager.UpdateAsync(user); // Save the secret key to the database

            // Step 3: Send the secret key via email
            var emailSubject = "Your 2FA Secret Key";
            var emailMessage = $"Here is your 2FA secret key: {secretKey}. Please use it in your authenticator app.";
            await _twoFactorAuthService.SendSecretKeyToEmail(user.Email, secretKey, _emailSender);

            // Inform the user that the secret key has been sent to their email
            EmailMessage = "The secret key has been sent to your email. Please use it in your authenticator app.";

            return Page(); // Return to the page
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // Retrieve the stored Google Authenticator secret key
            var secretKey = user.GoogleAuthenticatorSecret;

            // Verify the OTP entered by the user
            var isValidOtp = _twoFactorAuthService.ValidateOTP(secretKey, TwoFactorCode);
            if (isValidOtp)
            {
                // Enable 2FA after successful verification
                await _userManager.SetTwoFactorEnabledAsync(user, true);
                return RedirectToPage("/Index");
            }

            ModelState.AddModelError(string.Empty, "Invalid verification code.");
            return Page();  // Show the error message if the OTP is invalid
        }
    }
}
