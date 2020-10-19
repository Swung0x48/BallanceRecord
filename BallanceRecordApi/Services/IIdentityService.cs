using System.Threading.Tasks;
using BallanceRecordApi.Domain;

namespace BallanceRecordApi.Services
{
    public interface IIdentityService
    {
        Task<AuthenticationResult> RegisterAsync(string email, string password);
        Task<AuthenticationResult> LoginAsync(string email, string password);
        Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken);
        Task<AuthenticationResult> ConfirmEmailAsync(string userId, string token);
    }
}