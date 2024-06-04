using ChatApp.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Features.Accounts.Queries.GetAllUsers
{
    public class MemberDto
    {
        public string? Id { get; set; }
        public string? UserName { get; set; }
        public int Age { get; set; }
        public string? PhotoUrl { get; set; }
        public string? KnownAs { get; set; }
        public DateTime Created { get; set; } 
        public DateTime LastActive { get; set; }
        public string? Gender { get; set; }
        public string? Introduction { get; set; }
        public string? LookingFor { get; set; }
        public string? Interests { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public ICollection<PhotoDto> Photos { get; set; }
    }
}
