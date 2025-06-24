using BirthdayNotifier.models;
using Microsoft.EntityFrameworkCore;


namespace BirthdayNotifier.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Users> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("Users");
            base.OnModelCreating(modelBuilder);
        }


    }
}
