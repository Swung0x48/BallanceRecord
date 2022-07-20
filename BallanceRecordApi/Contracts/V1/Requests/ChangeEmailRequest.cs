using System.ComponentModel.DataAnnotations;

namespace BallanceRecordApi.Contracts.V1.Requests
{
    public class ChangeEmailRequest
    {
        [EmailAddress]
        public string Email { get; set; }
    }
}