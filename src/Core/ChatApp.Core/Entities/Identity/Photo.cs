using ChatApp.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Domain.Entities.Identity
{
    public class Photo : BaseEntity
    {
        public string Url { get; set; }
        public bool IsMain { get; set; }
        public string? PublishId { get; set; }

        public string AppUserId { get; set; } 
        public virtual AppUser AppUser { get; set; }
    }
}
