// Services/SendGridEmailSender.cs
using RentalManagementService.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net.Mail;

namespace RentalManagementService.Services
{
    public class SendGridEmailSender : IEmailSender
    {
        private readonly string _apiKey;
        private readonly string _fromEmail;
        private readonly string _fromName;

        public SendGridEmailSender(IConfiguration cfg)
        {
            _apiKey = cfg["Email:SendGrid:ApiKey"] ?? "";
            _fromEmail = cfg["Email:From:Email"] ?? "no-reply@yourdomain";
            _fromName = cfg["Email:From:Name"] ?? "RentalMgmt";
        }

        public async Task SendAsync(string toEmail, string subject, string htmlBody, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(_apiKey))
                throw new InvalidOperationException("SendGrid API key missing.");

            var client = new SendGridClient(_apiKey);
            var msg = new SendGridMessage
            {
                From = new EmailAddress(_fromEmail, _fromName),
                Subject = subject,
                HtmlContent = htmlBody
            };
            msg.AddTo(new EmailAddress(toEmail));
            var res = await client.SendEmailAsync(msg, ct);
            if ((int)res.StatusCode >= 400)
            {
                var body = await res.Body.ReadAsStringAsync(ct);
                throw new Exception($"SendGrid send failed: {(int)res.StatusCode} {body}");
            }
        }
    }
}
