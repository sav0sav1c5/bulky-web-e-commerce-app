using BulkyWebRazor.Models;
using Microsoft.EntityFrameworkCore;

namespace BulkyWebRazor.Data
{
    public class ApplicationDbContext : DbContext
    {
        // Instance of DbContext
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        // Db Set
        public DbSet<Category> Categories { get; set; }

        // Table Seed
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Action", DisplayOrder = 1 },
                new Category { Id = 2, Name = "SciFi", DisplayOrder = 2 },
                new Category { Id = 3, Name = "Horor", DisplayOrder = 3 },
                new Category { Id = 4, Name = "Romance", DisplayOrder = 4 },
                new Category { Id = 5, Name = "Adventure", DisplayOrder = 5 }
                );
        }
    }
}
