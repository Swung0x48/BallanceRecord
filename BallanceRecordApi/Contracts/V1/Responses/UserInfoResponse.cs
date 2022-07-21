namespace BallanceRecordApi.Contracts.V1.Responses;

public class UserInfoResponse
{
    public string Username { get; set; }
    public string Email { get; set; }
    public bool EmailConfirmed { get; set; }
}