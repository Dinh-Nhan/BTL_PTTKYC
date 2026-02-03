using System.Net;
using System.Net.Mail;

namespace backend.Service.implementations
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendOtpAsync(string toEmail, string otp)
        {
            var emailSettings = _config.GetSection("EmailSettings");

            var host = emailSettings["Host"];
            var fromEmail = emailSettings["From"];
            var displayName = emailSettings["DisplayName"];
            var password = emailSettings["Password"];
            var port = int.Parse(emailSettings["Port"]!);

            // 🛑 Check lỗi config sớm
            if (string.IsNullOrEmpty(host) ||
                string.IsNullOrEmpty(fromEmail) ||
                string.IsNullOrEmpty(password))
            {
                throw new Exception("EmailSettings configuration is missing");
            }

            var client = new SmtpClient(host, port)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail, password)
            };

            var message = new MailMessage
            {
                From = new MailAddress(fromEmail, displayName),
                Subject = "Confirm Your Hotel Booking",
                Body = $"""
                <h2>English Center</h2>
                <p>Your verification code is:</p>
                <h1>{otp}</h1>
                <p>This code expires in 5 minutes.</p>
            """,
                IsBodyHtml = true
            };

            message.To.Add(toEmail);
            await client.SendMailAsync(message);
        }
    }
}
