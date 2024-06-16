using ChatApp.Domain.Entities.Identity;

namespace ChatApp.Domain.Entities
{
    public class UserLike
    {
        public string SourceUserId { get; set; }
        public AppUser SourceUser { get; set; }
        public string LikedUserId { get; set; }
        public AppUser LikedUser { get; set; } 
    }
}
