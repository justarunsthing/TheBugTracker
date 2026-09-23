using SendGrid;
using TheBugTracker.Models;
using SendGrid.Helpers.Mail;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace TheBugTracker.Services
{
    public class SendGridService : IEmailSender, IEmailSender<ApplicationUser>
    {
        private readonly string _sendGridKey;
        private readonly string _fromAddress;
        private readonly string _fromName;

        public SendGridService(IConfiguration config)
        {
            // IConfiguration by default looks in Environment variables/user secrets/appsettings
            _sendGridKey = config["SendGridKey"] ?? Environment.GetEnvironmentVariable("SendGridKey")
                ?? throw new InvalidOperationException("SendGridKey not found in config!");

            _fromAddress = config["SendGridEmail"] ?? Environment.GetEnvironmentVariable("SendGridEmail")
                ?? throw new InvalidOperationException("SendGridEmail not found in config!");

            _fromName = config["SendGridName"] ?? Environment.GetEnvironmentVariable("SendGridName")
                ?? throw new InvalidOperationException("SendGridName not found in config!");
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            SendGridClient client = new(_sendGridKey);
            EmailAddress from = new(_fromAddress, _fromName);

            // Matches & removes any HTML tags as we don't know if user can display system generated HTML
            string plainTextContent = Regex.Replace(htmlMessage, "<[a-zA-Z/].*?>", "").Trim();
            List<string> emails = [.. email.Split(";", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)];
            List<EmailAddress> addresses = [.. emails.Select(e => new EmailAddress(e))];
            SendGridMessage message = MailHelper.CreateSingleEmailToMultipleRecipients(from, addresses, subject, plainTextContent, htmlMessage);

            Response response = await client.SendEmailAsync(message);

            if (response.IsSuccessStatusCode == false)
            {
                Console.WriteLine("******** EMAIL SERVICE ERROR ********");
                Console.WriteLine(await response.Body.ReadAsStringAsync());
                Console.WriteLine("******** EMAIL SERVICE ERROR ********");

                throw new BadHttpRequestException("SendGrid email could not be sent!");
            }
        }

        public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink) =>
            SendEmailAsync(email, "Confirm your email", $"Please confirm your account by <a href='{confirmationLink}'>clicking here</a>.");

        public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink) =>
            SendEmailAsync(email, "Reset your password", $"Please reset your password by <a href='{resetLink}'>clicking here</a>.");

        public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode) =>
            SendEmailAsync(email, "Reset your password", $"Please reset your password using the following code: {resetCode}");
    }
}