using ChatApp.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.Persistence.Configurations.DataSeed
{
    public class UserPhotoSeed : IEntityTypeConfiguration<Photo>
    {
        public void Configure(EntityTypeBuilder<Photo> builder)
        {
            builder.HasData(
                new Photo() { Id = 1, AppUserId = "2b7872a9-6c31-4b56-9b9d-2932d4d7d2d0", IsActive = true, IsMain = true, Url = "https://xsgames.co/randomusers/assets/avatars/male/1.jpg" },
                new Photo() { Id = 2, AppUserId = "2df43ac6-0edc-4d04-bc1c-54a8cca94583", IsActive = true, IsMain = true, Url = "https://xsgames.co/randomusers/assets/avatars/male/2.jpg" },
                new Photo() { Id = 3, AppUserId = "a735f1fc-c79f-4734-b34f-4c738406c275", IsActive = true, IsMain = false, Url = "https://xsgames.co/randomusers/assets/avatars/male/3.jpg" },
                new Photo() { Id = 4, AppUserId = "d6290192-24bb-4238-a984-9cf8bac6af05", IsActive = true, IsMain = false, Url = "https://xsgames.co/randomusers/assets/avatars/male/4.jpg" }
                );
        }
    }
}
