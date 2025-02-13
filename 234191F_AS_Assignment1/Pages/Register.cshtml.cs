using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using _234191F_AS_Assignment1.ViewModels;
using _234191F_AS_Assignment1.Model;
using System.IO;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;

namespace _234191F_AS_Assignment1.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<Register> _userManager;
        private readonly SignInManager<Register> _signInManager;
        private readonly IWebHostEnvironment _environment;
        private readonly IDataProtector _protector;

        [BindProperty]
        public RegisterViewModel RModel { get; set; }

        public RegisterModel(UserManager<Register> userManager, SignInManager<Register> signInManager,
                             IWebHostEnvironment environment, IDataProtectionProvider protectionProvider) // Inject TwoFactorAuthService
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _environment = environment;
            _protector = protectionProvider.CreateProtector("CreditCardProtection");
        
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Check if Password and ConfirmPassword match
            if (RModel.Password != RModel.ConfirmPassword)
            {
                ModelState.AddModelError("RModel.ConfirmPassword", "Passwords do not match.");
                return Page();
            }

            // Encrypt the Credit Card Number
            string encryptedCreditCardNo = _protector.Protect(RModel.CreditCardNo);

            string photoPath = null;
            if (RModel.Photo != null)
            {
                // Save photo to the server
                string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + RModel.Photo.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await RModel.Photo.CopyToAsync(fileStream);
                }

                photoPath = "/uploads/" + uniqueFileName;
            }

            // Create the Register object based on the ViewModel data
            var user = new Register
            {
                UserName = RModel.Email,
                Email = RModel.Email,
                FullName = RModel.FullName,
                CreditCardNo = encryptedCreditCardNo,
                Gender = RModel.Gender,
                MobileNumber = RModel.MobileNumber,
                DeliveryAddress = RModel.DeliveryAddress,
                AboutMe = RModel.AboutMe,
                PhotoPath = photoPath // Store the file path for the photo
            };

            // Create the user using UserManager
            var result = await _userManager.CreateAsync(user, RModel.Password);

            if (result.Succeeded)
            {
                // Step 1: Generate the 2FA secret key
                

                // Step 2: Store the generated secret key in the user record
               //user.GoogleAuthenticatorSecret = secretKey;
                await _userManager.UpdateAsync(user); // Save the secret key to the database



                TempData["RegistrationSuccess"] = "Registration successful! Please log in.";
                return RedirectToPage("/Login");
            }

            // Add errors to ModelState if user creation failed
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }

        private bool IsPasswordStrong(string password)
        {
            return password.Length >= 8 &&
                   password.Any(char.IsUpper) &&
                   password.Any(char.IsDigit) &&
                   password.Any(ch => !char.IsLetterOrDigit(ch)); // Check for special characters
        }

        // Optional: Decrypt the credit card number when needed (e.g., display on the profile)
        private string DecryptCreditCard(string encryptedCardNumber)
        {
            return _protector.Unprotect(encryptedCardNumber);
        }
    }
}

