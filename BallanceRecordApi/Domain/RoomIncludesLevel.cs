using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BallanceRecordApi.Domain;

[Index(nameof(RoomId), nameof(Order), IsUnique = true)]
public class RoomIncludesLevel
{
    [Key]
    public Guid Id { get; set; }
    public Guid RoomId { get; set; }
    // [Key, Column(Order = 1)]
    public string LevelHash { get; set; }
    
    public uint Order { get; set; }
    
    [ForeignKey(nameof(RoomId))]
    public Room Room { get; set; }
    [ForeignKey(nameof(LevelHash))]
    public Level Level { get; set; }
}