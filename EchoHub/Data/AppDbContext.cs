using Microsoft.EntityFrameworkCore;
using EchoHub.Models;

namespace EchoHub.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
           : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }

        public DbSet<EwasteItem> EwasteItems { get; set; }
    }
}
