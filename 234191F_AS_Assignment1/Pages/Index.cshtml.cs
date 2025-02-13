using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using _234191F_AS_Assignment1.Model;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;

namespace _234191F_AS_Assignment1.Pages
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<Register> _userManager;
        private SignInManager<Register> _signInManager;
        private readonly IDataProtector _protector;

        // User information to be displayed on the page
        public string UserEmail { get; set; }
        public string FullName { get; set; }
        public string MobileNumber { get; set; }
        public string DeliveryAddress { get; set; }
        public string AboutMe { get; set; }
        public string DecryptedCreditCardNo { get; set; }
        public string PhotoPath { get; set; }

        public IndexModel(UserManager<Register> userManager, SignInManager<Register> signInManager, IDataProtectionProvider protectionProvider)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _protector = protectionProvider.CreateProtector("CreditCardProtection"); // Key for encryption/decryption
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // Retrieve the user from the session (or use HttpContext.User if signed in)
            var userEmail = HttpContext.Session.GetString("UserEmail");

            // Check if session data is valid
            if (string.IsNullOrEmpty(userEmail))
            {
                // Redirect to login page if session has expired
                await _signInManager.SignOutAsync();
                return RedirectToPage("/Login");
            }

            var user = await _userManager.FindByEmailAsync(userEmail);

            // If the user exists, set the properties to display user info
            if (user != null)
            {
                UserEmail = user.Email;
                FullName = user.FullName;
                MobileNumber = user.MobileNumber;
                DeliveryAddress = user.DeliveryAddress;
                AboutMe = user.AboutMe;

                // Decrypt the sensitive data
                if (!string.IsNullOrEmpty(user.CreditCardNo))
                {
                    DecryptedCreditCardNo = _protector.Unprotect(user.CreditCardNo);
                }

                // Photo path (can be used to display the image)
                PhotoPath = user.PhotoPath;
            }

            // If user doesn't exist or session data is invalid, redirect to login page
            return Page();
        }
    }
}
