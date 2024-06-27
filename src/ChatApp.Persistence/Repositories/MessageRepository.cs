using AutoMapper;
using AutoMapper.QueryableExtensions;
using ChatApp.Application.Features.Messages.Queries.GetMessageForUser;
using ChatApp.Application.Helpers;
using ChatApp.Application.Persistence.Contracts;
using ChatApp.Core.Entities;
using ChatApp.Domain.Entities;
using ChatApp.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Persistence.Repositories
{
    public class MessageRepository : GenericRepository<Message>, IMessageRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public MessageRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<PagedList<MessageDto>> GetMessagesForUserAsync(MessageParams messageParams)
        {
            var query = _dbContext.Messages
                .OrderByDescending(x => x.MessageSend)
                .ProjectTo<MessageDto>(_mapper.ConfigurationProvider)
                .AsQueryable();

            query = messageParams.Container switch
            {
                "Inbox" => query.Where(x => x.RecipientUserName == messageParams.UserName && x.RecipientDeleted == false),
                "Outbox" => query.Where(x => x.RecipientUserName == messageParams.UserName && x.SenderDeleted == false),
                _ => query.Where(x => x.RecipientUserName == messageParams.UserName && x.RecipientDeleted == false && x.DateRead == null),
            };

            return await PagedList<MessageDto>.CreateAsync(query, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDto>> GetMessagesIsReadAsync(string currentUserName, string recipientUserName)
        {
            // Retrieve messages asynchronously with necessary includes
            var messages = await _dbContext.Messages
                    .Where(x => (x.Recipient.UserName == currentUserName && !x.RecipientDeleted && x.Sender.UserName == recipientUserName && !x.SenderDeleted) ||
                        (x.Recipient.UserName == recipientUserName && !x.RecipientDeleted && x.Sender.UserName == currentUserName && !x.SenderDeleted))
                    .OrderBy(x => x.MessageSend)
                    .ProjectTo<MessageDto>(_mapper.ConfigurationProvider)
                    .ToListAsync();

            // Retrieve unread messages for the current user
            var unreadMessages = messages
                .Where(x => x.DateRead == null && x.RecipientUserName == currentUserName).ToList();

            if (unreadMessages.Any())
            {
                // Mark messages as read
                foreach (var message in unreadMessages)
                {
                    message.DateRead = DateTime.Now;
                }

                // Save changes in a batch
                //await _dbContext.SaveChangesAsync();
            }

            return messages;
        }


        public async Task DeleteMessageAsync(Message message)
        {
            _dbContext.Messages.Remove(message);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Message?> GetMessageByIdAsync(int id)
        {
            var message = await _dbContext.Messages
                .Include(x => x.Sender)
                .Include(x => x.Recipient)
                .FirstOrDefaultAsync(x => x.Id == id);

            return message;
        }

        public void AddGroup(Group messageGroup)
        {
            _dbContext.Groups.Add(messageGroup);
        }

        public void AddMessage(Message message)
        {
            _dbContext.Messages.Add(message);
        }
        public void RemoveMessage(Message message)
        {
            _dbContext.Messages.Remove(message);
        }

        public void RemoveConnection(Connection connection)
        {
            _dbContext.Connections.Remove(connection);
        }

        public async Task<Connection?> GetConnectionAsync(string connectionId)
        {
            return await _dbContext.Connections.FindAsync(connectionId);
        }

        public async Task<Group?> GetMessageGroupAsync(string groupName)
        {
            return await _dbContext.Groups.Include(x => x.Connections).FirstOrDefaultAsync(x => x.Name == groupName);
        }

        public async Task<Group?> GetGroupForConnectionAsync(string connectionId)
        {
            return await _dbContext.Groups
                .Include(c => c.Connections)
                .Where(c => c.Connections.Any(x => x.ConnectionId == connectionId))
                .FirstOrDefaultAsync();
        }
    }
}
