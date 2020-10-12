using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace BallanceRecordApi.Domain
{
    public class Record
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
        
        [ForeignKey(nameof(UserId))]
        public IdentityUser User { get; set; }
    }
}
