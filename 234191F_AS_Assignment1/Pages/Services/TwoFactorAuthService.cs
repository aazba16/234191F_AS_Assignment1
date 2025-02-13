using OtpNet;
using System;
using Microsoft.AspNetCore.Identity.UI.Services;


public class TwoFactorAuthService
{
    // Generate the 2FA Secret Key for TOTP (Time-based One-Time Password)
    public string GenerateSecretKey()
    {
        var key = KeyGeneration.GenerateRandomKey(20); // 20-byte key for TOTP
        return Base32Encoding.ToString(key); // Convert it to Base32 encoding for easy storage
    }

    // Generate OTP based on the secret key and current time (TOTP)
    public string GenerateOTP(string secretKey)
    {
        var totp = new Totp(Base32Encoding.ToBytes(secretKey)); // TOTP algorithm
        return totp.ComputeTotp(); // Generate OTP (it’s valid for 30 seconds)
    }

    // Validate OTP entered by the user
    public bool ValidateOTP(string secretKey, string userOtp)
    {
        var totp = new Totp(Base32Encoding.ToBytes(secretKey));
        return totp.VerifyTotp(userOtp, out _, new VerificationWindow(2, 2)); // This compares the OTP within a 2-window offset
    }


    // Send the Secret Key via Email
    public async Task SendSecretKeyToEmail(string email, string secretKey, IEmailSender emailSender)
    {
        var emailSubject = "Your 2FA Secret Key";
        var emailMessage = $"Here is your 2FA secret key: {secretKey}. Please use it in your authenticator app.";
        await emailSender.SendEmailAsync(email, emailSubject, emailMessage);
    }
}
