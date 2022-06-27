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

public class Session
{
    [Key]
    public Guid Id { get; set; }
    public string Remark { get; set; }
    public Status Status { get; set; }
    public string UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public IdentityUser User { get; set; }
}