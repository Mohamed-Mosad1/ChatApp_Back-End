using ChatApp.Core.Common;
using ChatApp.Domain.Entities.Identity;

namespace ChatApp.Core.Entities
{
    public class Message : BaseEntity
    {
        // Sender's details
        public string? SenderId { get; set; }
        public string SenderUserName { get; set; } = null!;
        public AppUser Sender { get; set; }

        // Recipient's details
        public string? RecipientId { get; set; }
        public string RecipientUserName { get; set; } = null!;
        public AppUser Recipient { get; set; }

        // Message content
        public string Content { get; set; } = null!;

        // Timestamps
        public DateTime? DateRead { get; set; }
        public DateTime MessageSend { get; set; } = DateTime.Now;

        // Deletion flags
        public bool SenderDeleted { get; set; }
        public bool RecipientDeleted { get; set; }
    }
}
