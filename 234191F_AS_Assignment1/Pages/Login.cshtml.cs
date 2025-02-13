using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using _234191F_AS_Assignment1.ViewModels;
using _234191F_AS_Assignment1.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace _234191F_AS_Assignment1.Pages
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<Register> _signInManager;
        private readonly UserManager<Register> _userManager;
        private readonly AuthDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly reCAPTCHASettings _reCAPTCHASettings;
        private readonly IEmailSender _emailSender;  // Injected IEmailSender

        public string reCAPTCHASiteKey { get; set; }

        [BindProperty]
        public Login LModel { get; set; }

        public LoginModel(SignInManager<Register> signInManager, UserManager<Register> userManager, AuthDbContext dbContext,IConfiguration configuration,
                          IEmailSender emailSender)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _dbContext = dbContext;
            _configuration = configuration;
            _emailSender = emailSender;  // Assign the injected IEmailSender
        }

        public void OnGet()
        {
            reCAPTCHASiteKey = _configuration["reCAPTCHA:SiteKey"];
            // Check if the user is already logged in
            if (_signInManager.IsSignedIn(User))
            {
                // If logged in, redirect to the homepage
                Response.Redirect("/Index");
            }
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                // Verify reCAPTCHA response from the client-side
                var recaptchaResponse = Request.Form["recaptchaResponse"];
                var recaptchaSecretKey = _configuration["reCAPTCHA:SecretKey"];
                var client = new HttpClient();
                var verifyUrl = "https://www.google.com/recaptcha/api/siteverify";
                var requestContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("secret", recaptchaSecretKey),
                    new KeyValuePair<string, string>("response", recaptchaResponse)
                });

                var response = await client.PostAsync(verifyUrl, requestContent);
                var jsonResponse = await response.Content.ReadAsStringAsync();
                dynamic recaptchaResult = JsonConvert.DeserializeObject(jsonResponse);

                if (recaptchaResult.success != "true")
                {
                    ModelState.AddModelError(string.Empty, "reCAPTCHA verification failed. Please try again.");
                    return Page();
                }

                var result = await _signInManager.PasswordSignInAsync(LModel.Email, LModel.Password, LModel.RememberMe, lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(LModel.Email);

                    // Check if the user has 2FA enabled
                    if (await _userManager.GetTwoFactorEnabledAsync(user))
                    {
                        // Generate a 2FA token (can be sent via email or use an authenticator app)
                        var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Authenticator");

                        // Send the token via email (You can send via SMS too, if needed)
                        await _emailSender.SendEmailAsync(user.Email, "Your 2FA Code", $"Your 2FA code is: {token}");

                        // Redirect to the 2FA verification page
                        return RedirectToPage("/Account/Verify2FA");
                    }

                    // Get device info and IP address of the current login
                    var deviceInfo = Request.Headers["User-Agent"].ToString();
                    var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

                    // Retrieve the last active login record
                    var previousLogin = _dbContext.AuditLogs
                        .Where(log => log.UserId == user.Email && log.IsActive)
                        .OrderByDescending(log => log.Timestamp)
                        .FirstOrDefault();

                    // Handle multiple login scenarios
                    if (previousLogin != null && previousLogin.DeviceInfo != deviceInfo)
                    {
                        TempData["MultipleLogins"] = "You have logged in from a new device/browser. Please ensure your account security.";
                    }

                    // Create a new session entry
                    var sessionId = Guid.NewGuid().ToString(); // Generate a new session ID
                    var auditLog = new AuditLog
                    {
                        UserId = user.Email,
                        Action = "User Logged In",
                        Timestamp = DateTime.Now,
                        DeviceInfo = deviceInfo,
                        IpAddress = ipAddress,
                        SessionId = sessionId,
                        IsActive = true // Mark the session as active
                    };

                    _dbContext.AuditLogs.Add(auditLog);
                    await _dbContext.SaveChangesAsync();

                    // Store session details
                    HttpContext.Session.SetString("UserEmail", user.Email);
                    HttpContext.Session.SetString("UserName", user.UserName);

                    return RedirectToPage("Index");
                }
                else if (result.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "Your account is locked due to multiple failed login attempts. Please try again later.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }

            return Page();
        }







    }
}
