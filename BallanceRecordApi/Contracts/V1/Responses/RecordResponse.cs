using System;

namespace BallanceRecordApi.Contracts.V1.Responses
{
    public class RecordResponse
    {
        public Guid Id { get; set; }
        public string Remark { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        public string LevelHash { get; set; }
        public int Score { get; set; }
        public Guid? RoomId { get; set; }
        public double Duration { get; set; }
        public DateTime TimeCreated { get; set; }
        public DateTime TimeModified { get; set; }
    }
}