using AutoMapper;
using AutoMapper.QueryableExtensions;
using ChatApp.Application.Features.Messages.Queries.GetMessageForUser;
using ChatApp.Application.Helpers;
using ChatApp.Application.Persistence.Contracts;
using ChatApp.Core.Entities;
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
            var query = _dbContext.Messages/*.Include(x => x.Sender).Include(x => x.Recipient)*/
                //.AsNoTracking()
                .OrderByDescending(x => x.MessageSend).AsQueryable();

            query = messageParams.Container switch
            {
                "Inbox" => query.Where(x => x.Recipient.UserName == messageParams.UserName && x.RecipientDeleted == false),
                "Outbox" => query.Where(x => x.Sender.UserName == messageParams.UserName && x.SenderDeleted == false),
                _ => query.Where(x => x.Recipient.UserName == messageParams.UserName && x.RecipientDeleted == false && x.DateRead == null),
            };

            var messageQuery = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);

            return await PagedList<MessageDto>.CreateAsync(messageQuery, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDto>> GetMessagesIsReadAsync(string currentUserName, string recipientUserName)
        {
            // Retrieve messages asynchronously with necessary includes
            var messages = await _dbContext.Messages
                .Include(x => x.Sender).ThenInclude(x => x.Photos)
                .Include(x => x.Recipient).ThenInclude(x => x.Photos)
                .Where(x => (x.Recipient.UserName == currentUserName && !x.RecipientDeleted && x.Sender.UserName == recipientUserName && !x.SenderDeleted) ||
                            (x.Recipient.UserName == recipientUserName && !x.RecipientDeleted && x.Sender.UserName == currentUserName && !x.SenderDeleted))
                .OrderBy(x => x.MessageSend)
                .ToListAsync();

            // Retrieve unread messages for the current user
            var unreadMessages = messages.Where(x => x.DateRead == null && x.Recipient.UserName == currentUserName).ToList();

            if (unreadMessages.Any())
            {
                // Mark messages as read
                foreach (var message in unreadMessages)
                {
                    message.DateRead = DateTime.Now;
                }
                // Save changes in a batch
                await _dbContext.SaveChangesAsync();
            }

            return _mapper.Map<IEnumerable<MessageDto>>(messages);
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
                //.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            return message;
        }
    }
}
