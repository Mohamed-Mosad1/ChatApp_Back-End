using ChatApp.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Persistence.Contracts
{
    public interface ITokenService
    {
        Task<string> CreateTokenAsync(AppUser user);
    }
}
