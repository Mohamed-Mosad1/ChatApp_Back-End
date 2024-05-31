using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Features.Messages.Command.AddMessage
{
    public class AddMessageDto
    {
        public int SenderId { get; set; }
        public int RecipientId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime? DateRead { get; set; }
    }
}
