using BallanceRecordApi.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BallanceRecordApi.Data
{
    public class DataContext : IdentityDbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        public DbSet<Record> Records { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Session> Sessions { get; set; }
    }
}
