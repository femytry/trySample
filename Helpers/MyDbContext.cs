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
    }
}