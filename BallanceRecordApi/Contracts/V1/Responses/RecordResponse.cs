using System;

namespace BallanceRecordApi.Contracts.V1.Responses
{
    public class RecordResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        public string MapHash { get; set; }
        public int Score { get; set; }
        public double Time { get; set; }
    }
}