using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Features.Likes.Command.AddOrRemoveLike
{
    public class LikeDto
    {
        public string Id { get; set; }
        public string? UserName { get; set; }
        public string? KnownAs { get; set; }
        public int Age { get; set; }
        public string? City { get; set;}
        public bool IsLiked { get; set; }
        public string? PhotoUrl { get; set; }
    }
}
