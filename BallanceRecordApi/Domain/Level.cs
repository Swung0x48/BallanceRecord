#nullable enable
using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace BallanceRecordApi.Domain;

[Index(nameof(Hash), IsUnique = true)]
public class Level
{
    [Key]
    public Guid Id { get; set; }
    public string Hash { get; set; }
    public string? Name { get; set; }
    public bool IsCustom { get; set; }
    public string? Author { get; set; }
    public string? Difficulty { get; set; }
    public string? Remark { get; set; }
}