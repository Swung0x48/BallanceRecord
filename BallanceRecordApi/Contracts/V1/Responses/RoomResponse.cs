using System;
using BallanceRecordApi.Domain;

namespace BallanceRecordApi.Contracts.V1.Responses
{
    public class RoomResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Status Status { get; set; }
        public string RoomHostUserId { get; set; }
        public string Remark { get; set; }
    }
}