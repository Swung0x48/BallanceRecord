using System;
using BallanceRecordApi.Domain;

namespace BallanceRecordApi.Contracts.V1.Responses
{
    public class BriefUser
    {
        public string UserId { get; set; }
        public string Username { get; set; }
    }
    public class RoomResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Status Status { get; set; }
        public BriefUser Host { get; set; }
        public string Remark { get; set; }
    }
}