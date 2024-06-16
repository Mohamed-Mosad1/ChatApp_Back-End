using ChatApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.Persistence.Configurations
{
    public class UserLikeConfigurations : IEntityTypeConfiguration<UserLike>
    {
        public void Configure(EntityTypeBuilder<UserLike> builder)
        {
            builder.HasKey(x => new { x.SourceUserId, x.LikedUserId });
            builder.HasOne(x=>x.SourceUser).WithMany(x=>x.LikeUser)
                .HasForeignKey(x=>x.SourceUserId).OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.LikedUser).WithMany(x => x.LikedByUser)
                .HasForeignKey(x => x.LikedUserId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
