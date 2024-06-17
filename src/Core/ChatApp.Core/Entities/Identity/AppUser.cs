﻿using ChatApp.Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace ChatApp.Domain.Entities.Identity
{
    public sealed class AppUser : IdentityUser
    {
        public DateTime DateOfBirth { get; set; }
        public string? KnownAs { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime LastActive { get; set; } = DateTime.Now;
        public string? Gender { get; set; }
        public string? Introduction { get; set; }
        public string? LookingFor { get; set; }
        public string? Interests { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public ICollection<Photo> Photos { get; set; } = new HashSet<Photo>();

        // Likes
        public ICollection<UserLike> LikeUser { get; set; } = new HashSet<UserLike>();
        public ICollection<UserLike> LikedByUser { get; set; } = new HashSet<UserLike>();

        // Messages
        public ICollection<Message> MessagesSent { get; set; } = new HashSet<Message>();
        public ICollection<Message> MessagesReceived { get; set; } = new HashSet<Message>();

    }
}
