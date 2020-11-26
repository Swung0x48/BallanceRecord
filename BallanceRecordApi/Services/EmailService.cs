using System.Threading.Tasks;
using BallanceRecordApi.Options;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;

namespace BallanceRecordApi.Services
{
    public class EmailService: IEmailService
    {
        private readonly EmailOptions _emailOptions;

        public EmailService(EmailOptions emailOptions)
        {
            _emailOptions = emailOptions;
        }
        
        public async Task SendAsync(string to, string subject, string html)
        {
            // create message
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_emailOptions.EmailAddress));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = html };

            // send email
            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_emailOptions.Host, _emailOptions.Port, _emailOptions.SecureSocketOptions);
            await smtp.AuthenticateAsync(_emailOptions.Username, _emailOptions.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}