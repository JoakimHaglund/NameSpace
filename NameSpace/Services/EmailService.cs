using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using NameSpace.Dtos;
using NameSpace.Models;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
using System.Xml.Linq;

namespace NameSpace.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;


        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }


        public async Task SendEmailAsync(string to, string subject, string body)
        {

            if (string.IsNullOrEmpty(to))
            {
                throw new ArgumentException("Recipient email cannot be null or empty.", nameof(to));
            }

            if (string.IsNullOrEmpty(subject))
            {
                throw new ArgumentException("Email subject cannot be null or empty.", nameof(subject));
            }

            if (string.IsNullOrEmpty(body))
            {
                throw new ArgumentException("Email body cannot be null or empty.", nameof(body));
            }
            // Fetch SMTP settings from configuration
            var smtpHost = _configuration["SMTP:Host"];
            var smtpPortValue = _configuration["SMTP:Port"];
            var smtpEmail = _configuration["SMTP:Email"];
            var smtpPassword = _configuration["SMTP:Password"];

            // Print config for debugging
            Console.WriteLine($"SMTP_Host: {smtpHost} SMTP_Port: {smtpPortValue} SMTP_Email: {smtpEmail}");

            // Check for null SMTP configuration values
            if (string.IsNullOrEmpty(smtpHost))
            {
                Console.WriteLine("SMTP host cannot be null or empty.");
                throw new InvalidOperationException("SMTP configuration is invalid");
            }
            if (string.IsNullOrWhiteSpace(smtpPortValue) || !int.TryParse(smtpPortValue, out var smtpPort))
            {
                Console.WriteLine("SMTP Port is not configured or is invalid.");
                throw new InvalidOperationException("SMTP configuration is invalid");
            }
            if (string.IsNullOrEmpty(smtpEmail))
            {
                Console.WriteLine("SMTP email cannot be null or empty.");
                throw new InvalidOperationException("SMTP configuration is invalid");
            }

            if (string.IsNullOrEmpty(smtpPassword))
            {
                Console.WriteLine("SMTP password cannot be null or empty.");
                throw new InvalidOperationException("SMTP configuration is invalid");
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("NameSpace", smtpEmail)); // Use the configured SMTP email
            message.To.Add(new MailboxAddress(null, to)); // Recipient email
            message.Subject = subject;
            message.Body = new TextPart("plain") { Text = body };

            using var client = new SmtpClient();
            try
            {
                // Connect to SMTP server
                await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls);

                // Authenticate with SMTP server
                await client.AuthenticateAsync(smtpEmail, smtpPassword);

                // Send the email
                await client.SendAsync(message);

                // Disconnect cleanly
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while sending email: {ex.Message}");
            }
        }

        public EmailTemplate? TemplateEmailVerification(User user, string token)
        {
            if (user.Email == null)
            {
                return null;
            }

            return new EmailTemplate
            {
                SendTo = user.Email,
                Subject = "Email verification",
                Body = @$"
                    Välkommen till NameSpace {user.UserName}!/n
                    För att aktivera ditt konto använd länken nedan./n 
                    http://192.168.50.9:5500/index.html?token={token}&email={user.Email}"
            };
        }
        public EmailTemplate? TemplatePartnerRequest(User requestingUser, User recivingUser, Guid token)
        {
            if (recivingUser.Email == null)
            {
                return null;
            }

            return new EmailTemplate
            {
                SendTo = recivingUser.Email,
                Subject = $"Partner förfrågan",
                Body = @$"
                    {requestingUser.UserName} vill bli din partner på NameSpace!
                    Klicka på länken nedan för att acceptera.
                    http://192.168.50.9:5500/index.html?partner-token={token}"
            };
        }
        public class EmailTemplate
        {
            public required string SendTo { get; set; }
            public required string Subject { get; set; }
            public required string Body { get; set; }
        }
    }
}