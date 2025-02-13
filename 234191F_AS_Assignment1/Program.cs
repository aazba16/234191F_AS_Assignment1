using _234191F_AS_Assignment1.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using _234191F_AS_Assignment1.Pages.Services;
using Microsoft.AspNetCore.Identity.UI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AuthConnectionString")));

builder.Services.AddIdentity<Register, IdentityRole>()
    .AddEntityFrameworkStores<AuthDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddRazorPages();

// Add reCAPTCHA configuration
builder.Services.Configure<reCAPTCHASettings>(builder.Configuration.GetSection("reCAPTCHA"));

// Configure Identity options
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 12;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
});

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 3;
    options.Lockout.AllowedForNewUsers = true;
});

// Add session service
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20); // Set session timeout
    options.Cookie.HttpOnly = true; // Ensures cookie can't be accessed by JavaScript
    options.Cookie.IsEssential = true; // Essential for session state
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;  // Ensures cookie is only sent over HTTPS
});


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Events.OnRedirectToLogin = context =>
        {
            // Prevent redirection if the user is not logged in
            context.Response.Redirect("/Login");
            return Task.CompletedTask;
        };
    });

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

builder.Services.AddScoped<AccountServices>(); // Register AccountService for Dependency Injection
builder.Services.AddSingleton<IEmailSender, EmailSender>();  // Using a custom email sender
builder.Services.AddSingleton<IEmailSender, SmtpEmailSender>(); // Replace with your own email sender service
// Register TwoFactorAuthService as a scoped service
builder.Services.AddScoped<TwoFactorAuthService>();




var app = builder.Build();

// Custom error handling middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    // This is for production
    app.UseExceptionHandler("/errors/500");  // Custom 500 error page
    app.UseHsts();  // Enforce HTTP Strict Transport Security
}

// Custom error pages for 404, 403, etc.
app.UseStatusCodePagesWithRedirects("/errors/{0}");


// Automatically delete AuditLogs upon application startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    // Clear AuditLogs table upon startup or rebuild
    dbContext.Database.ExecuteSqlRaw("DELETE FROM AuditLogs");
    dbContext.Database.Migrate();  // Apply migrations if necessary
}

// Add session middleware
app.UseSession();  // Make sure session is added to the middleware pipeline

app.UseAuthentication(); // Add authentication middleware
app.UseAuthorization();  // Add authorization middleware
app.UseRouting();
app.MapRazorPages();
app.UseStaticFiles();

app.Run();
