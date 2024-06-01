using ChatApp.Core.Common;
using ChatApp.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ChatApp.Persistence.DatabaseContext
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if(entry.State == EntityState.Modified)
                    entry.Entity.ModifiedDate = DateTime.UtcNow;
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        public DbSet<Message> Messages { get; set; }
    }
}
