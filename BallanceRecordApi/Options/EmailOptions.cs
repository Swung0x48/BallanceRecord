using MailKit.Security;

namespace BallanceRecordApi.Options
{
    public class EmailOptions
    {
        public string EmailAddress { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public SecureSocketOptions SecureSocketOptions { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}