using System.Threading.Tasks;

namespace BallanceRecordApi.Services
{
    public interface IEmailService
    {
        Task SendAsync(string from, string to, string subject, string html);
    }
}