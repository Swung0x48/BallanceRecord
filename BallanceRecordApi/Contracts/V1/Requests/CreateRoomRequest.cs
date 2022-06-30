using System;

namespace BallanceRecordApi.Contracts.V1.Requests;

public class CreateRoomRequest
{
    public string Name { get; set; }
    public string Remark { get; set; }
}