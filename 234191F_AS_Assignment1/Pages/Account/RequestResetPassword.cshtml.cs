using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using _234191F_AS_Assignment1.ViewModels;
using System.Threading.Tasks;
using _234191F_AS_Assignment1.Model;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace _234191F_AS_Assignment1.Pages.Account
{
    public class RequestResetPasswordModel : PageModel
    {
        private readonly UserManager<Register> _userManager;
        private readonly IEmailSender _emailSender;

        public RequestResetPasswordModel(UserManager<Register> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [BindProperty]
        public ResetPasswordRequestModel Input { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page(); // If validation fails, reload the page
            }

            var user = await _userManager.FindByEmailAsync(Input.Email);

            if (user == null)
            {
                // Don't reveal that the email does not exist
                return RedirectToPage("/Account/ResetPasswordConfirmation");
            }

            // Generate password reset token
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Create reset password URL
            var resetUrl = Url.Page(
                "/Account/ResetPassword",
                pageHandler: null,
                values: new { token = token, email = Input.Email },
                protocol: Request.Scheme);

            // Send email with reset link
            var subject = "Password Reset Request";
            var message = $"Please reset your password by clicking <a href='{resetUrl}'>here</a>";
            await _emailSender.SendEmailAsync(Input.Email, subject, message);

            // Redirect to a confirmation page after sending the reset email
            return RedirectToPage("/Account/ResetPasswordConfirmation");
        }

    }
}
