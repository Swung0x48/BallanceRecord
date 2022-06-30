using BallanceRecordApi.Domain;

namespace BallanceRecordApi.Contracts.V1.Requests;

public class RoomStateTransitionRequest
{
    public Status Status { get; set; }
}