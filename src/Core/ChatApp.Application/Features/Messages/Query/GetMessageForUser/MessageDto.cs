using ChatApp.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Features.Messages.Query.GetMessageForUser
{
    public class MessageDto
    {
        public int Id { get; set; }

        // Sender's details
        public string? SenderId { get; set; }
        public string? SenderUserName { get; set; }
        public string? SenderProfilePictureUrl { get; set; }

        // Recipient's details
        public string? RecipientId { get; set; }
        public string? RecipientUserName { get; set; }
        public string? RecipientProfilePictureUrl { get; set; }

        // Message content
        public string? Content { get; set; }

        // Timestamps
        public DateTime? DateRead { get; set; }
        public DateTime MessageSend { get; set; } 

    }
}
