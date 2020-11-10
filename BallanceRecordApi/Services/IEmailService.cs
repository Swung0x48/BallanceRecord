using System.Threading.Tasks;

namespace BallanceRecordApi.Services
{
    public interface IEmailService
    {
        Task SendAsync(string to, string subject, string html);
    }
}