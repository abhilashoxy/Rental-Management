// Services/SmtpEmailSender.cs
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using RentalManagementService.Interfaces;

namespace RentalManagementService.Services
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly IConfiguration _cfg;
        public SmtpEmailSender(IConfiguration cfg) => _cfg = cfg;

        public async Task SendAsync(string toEmail, string subject, string htmlBody, CancellationToken ct = default)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_cfg["Email:From:Name"] ?? "RentalMgmt",
                                                _cfg["Email:From:Email"] ?? "no-reply@ethereal.email"));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;
            message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

            using var client = new SmtpClient();
            var host = _cfg["Email:Smtp:Host"] ?? "smtp.ethereal.email";
            var port = int.Parse(_cfg["Email:Smtp:Port"] ?? "587");
            var user = _cfg["Email:Smtp:User"];
            var pass = _cfg["Email:Smtp:Pass"];

            // STARTTLS for port 587. If you use 465, use SecureSocketOptions.SslOnConnect.
            await client.ConnectAsync(host, port, SecureSocketOptions.StartTls, ct);

            if (!string.IsNullOrWhiteSpace(user))
                await client.AuthenticateAsync(user, pass, ct);

            await client.SendAsync(message, ct);
            await client.DisconnectAsync(true, ct);
        }
    }
}
