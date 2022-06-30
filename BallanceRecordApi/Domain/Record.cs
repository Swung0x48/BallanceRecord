using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace BallanceRecordApi.Domain
{
    public class Record
    {
        [Key]
        public Guid Id { get; set; }
        public string Remark { get; set; }
        public string UserId { get; set; }
        public Guid? RoomId { get; set; }
        public string MapHash { get; set; }
        public int Score { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTime TimeCreated { get; set; }
        public DateTime TimeModified { get; set; }
        public double BallSpeed { get; set; }
        public bool IsBouncing { get; set; }
        
        [ForeignKey(nameof(UserId))]
        public IdentityUser User { get; set; }
        [ForeignKey(nameof(RoomId))]
        public Room Room { get; set; }
    }
}
