using Microsoft.EntityFrameworkCore;
using trySample.Entities;

namespace trySample.Helpers
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options)
            : base(options)
        { }
        public DbSet<Sample> Samples{ get; set; }
        public DbSet<User> Users{ get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasAlternateKey(u => u.Username)
                .HasName("AlternateKey_Username");
        }
    }
}