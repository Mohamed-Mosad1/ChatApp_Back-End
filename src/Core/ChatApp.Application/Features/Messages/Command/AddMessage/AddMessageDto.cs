using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Features.Messages.Command.AddMessage
{
    public class AddMessageDto
    {
        public string? RecipientUserName { get; set; }
        public string? Content { get; set; } 
    }
}
