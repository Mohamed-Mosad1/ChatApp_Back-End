using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Helpers
{
    public class MessageParams : PaginationParams
    {
        public string? UserName { get; set; }
        public string Container { get; set; } = "UnRead";
    }
}
