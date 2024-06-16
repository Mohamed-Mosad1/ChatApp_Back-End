using ChatApp.Core.Common;
using ChatApp.Core.Entities;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ChatApp.Persistence.DatabaseContext
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            
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
        public DbSet<Photo> Photos { get; set; }
        public DbSet<UserLike> UserLikes { get; set; }
    }
}
