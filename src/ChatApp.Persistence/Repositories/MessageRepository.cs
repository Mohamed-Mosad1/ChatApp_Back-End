using ChatApp.Application.Persistence.Contracts;
using ChatApp.Core.Entities;
using ChatApp.Persistence.DatabaseContext;

namespace ChatApp.Persistence.Repositories
{
    public class MessageRepository : GenericRepository<Message>, IMessageRepository
    {
        public MessageRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
