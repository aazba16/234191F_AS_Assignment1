using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using _234191F_AS_Assignment1.Model;
using _234191F_AS_Assignment1.Pages.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace _234191F_AS_Assignment1.Pages.Account
{
    public class Verify2FAModel : PageModel
    {
        private readonly UserManager<Register> _userManager;
        private readonly TwoFactorAuthService _twoFactorAuthService;
        private readonly SignInManager<Register> _signInManager;
        private readonly AuthDbContext _dbContext;  // Add the DbContext

        public Verify2FAModel(UserManager<Register> userManager,
            TwoFactorAuthService twoFactorAuthService,
            SignInManager<Register> signInManager,
            AuthDbContext dbContext)  // Inject the DbContext
        {
            _userManager = userManager;
            _twoFactorAuthService = twoFactorAuthService;
            _signInManager = signInManager;
            _dbContext = dbContext;  // Assign the DbContext
        }

        [BindProperty]
        public string TwoFactorCode { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || !await _userManager.GetTwoFactorEnabledAsync(user))
            {
                return RedirectToPage("/Login");
            }
            return Page();
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

            // Validate the OTP entered by the user
            var isValidOtp = _twoFactorAuthService.ValidateOTP(secretKey, TwoFactorCode);

            if (isValidOtp)
            {
                // Sign the user in and create a session
                await _signInManager.SignInAsync(user, isPersistent: true);

                // Create an Audit Log entry for the login
                var sessionId = Guid.NewGuid().ToString();  // Generate a new session ID
                var auditLog = new AuditLog
                {
                    UserId = user.Email,
                    Action = "User Logged In",
                    Timestamp = DateTime.Now,
                    DeviceInfo = Request.Headers["User-Agent"].ToString(),
                    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                    SessionId = sessionId,
                    IsActive = true // Mark the session as active
                };

                // Add the audit log and save
                _dbContext.AuditLogs.Add(auditLog);
                await _dbContext.SaveChangesAsync();

                // Store session details
                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetString("UserName", user.UserName);

                // Redirect to the user's home page or dashboard after successful 2FA
                return RedirectToPage("/Index");
            }

            ModelState.AddModelError(string.Empty, "Invalid verification code.");
            return Page();
        }
    }
}
