#nullable enable
namespace BallanceRecordApi.Contracts.V1.Requests
{
    public class CreateRecordRequest
    {
        public string Remark { get; set; }
        public int Score { get; set; }
        public string? RoomId { get; set; }
        public double Duration { get; set; }
        public string LevelHash { get; set; }
        public double BallSpeed { get; set; }
        public bool IsBouncing { get; set; }
    }
}