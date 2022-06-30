using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace BallanceRecordApi.Domain;

public enum Status
{
    Created,
    Running,
    Ended
}

public class Room
{
    [Key]
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Remark { get; set; }
    public Status Status { get; set; }
    public string RoomHostUserId { get; set; }
    [ForeignKey(nameof(RoomHostUserId))]
    public IdentityUser User { get; set; }
}