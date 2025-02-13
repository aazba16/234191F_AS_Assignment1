using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace _234191F_AS_Assignment1.Pages.Services
{
    public class SmtpEmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            using (var client = new SmtpClient("smtp.gmail.com"))
            {
                client.Port = 587;  // Or the appropriate port
                client.Credentials = new NetworkCredential("ayuraaman0@gmail.com", "ezsp fiha pogo gdvg");
                client.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("ayuraaman0@gmail.com"),
                    Subject = subject,
                    Body = htmlMessage,
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(email);
                await client.SendMailAsync(mailMessage);
            }
        }
    }
}
