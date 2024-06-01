using ChatApp.Core.Common;
using ChatApp.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Domain.Entities
{
    public class Photo : BaseEntity
    {
        public string Url { get; set; } = null!;
        public bool IsMain { get; set; }
        public string PublishId { get; set; } = null!;

        public string AppUserId { get; set; } = null!;
        public virtual AppUser AppUser { get; set; }
    }
}
