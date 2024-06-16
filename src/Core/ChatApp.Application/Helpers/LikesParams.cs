using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Helpers
{
    public class LikesParams : PaginationParams
    {
        public string? UserId { get; set; }
        public string? Predicate { get; set; }
    }
}
