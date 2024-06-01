using ChatApp.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Core.Entities
{
    public class Message : BaseEntity
    {
        public int SenderId { get; set; }
        public string SenderUserName { get; set; } = null!;

        public int RecipientId { get; set; }
        public string RecipientUserName { get; set; } = null!;

        public string Content { get; set; } = null!;

        public DateTime? DateRead { get; set; }
        public DateTime MessageSend { get; set; } = DateTime.UtcNow;

        public bool SenderDeleted { get; set; }
        public bool RecipientDeleted { get; set; }
    }
}
