using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BallanceRecordApi.Domain
{
    public class Record
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
