using System.Collections.Generic;

namespace BallanceRecordApi.Contracts.V1.Responses
{
    public class ErrorResponse
    {
        public List<ErrorModel> Error { get; set; } = new List<ErrorModel>();
    }
}