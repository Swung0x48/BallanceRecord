using System.Collections.Generic;

namespace BallanceRecordApi.Domain
{
    public class AuthenticationResult
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public bool Success { get; set; }
        public IEnumerable<string> Messages { get; set; }
    }
}