using System.Collections.Generic;

namespace BallanceRecordApi.Contracts.V1.Responses
{
    public class AuthFailResponse
    {
        public IEnumerable<string> Errors { get; set; }
    }
}