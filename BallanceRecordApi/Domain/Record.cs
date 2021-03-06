﻿using System;
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
        public string MapHash { get; set; }
        public int Score { get; set; }
        public double Time { get; set; }
        
        [ForeignKey(nameof(UserId))]
        public IdentityUser User { get; set; }
    }
}
