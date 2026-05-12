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
        public DbSet<EchoHub.Models.Collection> Collections { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<EwasteItem>()
            .Property(e => e.AmountPaid)
            .HasPrecision(18, 2);
            // Force mapping for the missing tables
            modelBuilder.Entity<Category>().ToTable("Categories");
            modelBuilder.Entity<EwasteItem>().ToTable("EwasteItems");
            modelBuilder.Entity<Collection>().ToTable("Collections");
        }
    }
}