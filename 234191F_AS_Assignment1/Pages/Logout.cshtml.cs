using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using _234191F_AS_Assignment1.Model;
using System;

namespace _234191F_AS_Assignment1.Pages
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<Register> _signInManager;
        private readonly UserManager<Register> _userManager;
        private readonly AuthDbContext _dbContext;

        public LogoutModel(SignInManager<Register> signInManager, UserManager<Register> userManager, AuthDbContext dbContext)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _dbContext = dbContext;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                var currentSession = _dbContext.AuditLogs
                    .Where(log => log.UserId == user.Email && log.IsActive)
                    .FirstOrDefault();

                if (currentSession != null)
                {
                    // Mark the session as inactive
                    currentSession.IsActive = false;
                    _dbContext.AuditLogs.Update(currentSession);
                    await _dbContext.SaveChangesAsync();
                }

                // Create an audit log for the logout action
                var auditLog = new AuditLog
                {
                    UserId = user.Email,
                    Action = "User Logged Out",
                    Timestamp = DateTime.Now,
                    DeviceInfo = Request.Headers["User-Agent"].ToString(),
                    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                    SessionId = currentSession?.SessionId, // Use the session ID from the active session
                    IsActive = false
                };

                // Save the audit log
                _dbContext.AuditLogs.Add(auditLog);
                await _dbContext.SaveChangesAsync();
            }

            await _signInManager.SignOutAsync();
            HttpContext.Session.Clear(); // Clear the session
            Response.Cookies.Delete("YourCookieName"); // Delete any relevant cookies

            return RedirectToPage("/Login");
        }


    }
}
