using System;

namespace BallanceRecordApi.Contracts.V1.Requests
{
    public class CreateRecordRequest
    {
        public string Name { get; set; }
        public int Score { get; set; }
        public double Time { get; set; }
    }
}