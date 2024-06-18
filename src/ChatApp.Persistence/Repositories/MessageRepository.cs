using AutoMapper;
using AutoMapper.QueryableExtensions;
using ChatApp.Application.Features.Messages.Query.GetMessageForUser;
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
            IQueryable<Message> query = _dbContext.Messages
                .AsNoTracking()
                .OrderByDescending(x => x.MessageSend);

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
            // Retrieve messages asynchronously
            var messages = await _dbContext.Messages
                .Include(x => x.Sender).ThenInclude(x => x.Photos)
                .Include(x => x.Recipient).ThenInclude(x => x.Photos)
                .AsNoTracking()
                .Where(x => (x.Recipient.UserName == currentUserName && x.Sender.UserName == recipientUserName && x.RecipientDeleted == false)
                         || (x.Recipient.UserName == recipientUserName && x.Sender.UserName == currentUserName && x.SenderDeleted == false))
                .OrderByDescending(x => x.MessageSend)
                .ProjectTo<MessageDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            // Retrieve unread messages for the current user
            var unReadMessages = await _dbContext.Messages
                .Where(x => x.DateRead == null && x.Recipient.UserName == currentUserName)
                .ToListAsync();

            if (unReadMessages.Any())
            {
                // Mark messages as read and save changes in a batch
                foreach (var message in unReadMessages)
                {
                    message.DateRead = DateTime.Now;
                }
                _dbContext.Messages.UpdateRange(unReadMessages);
                await _dbContext.SaveChangesAsync();
            }

            return messages;
        }

        public async Task DeleteMessageAsync(Message message)
        {
            _dbContext.Messages.Remove(message);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Message> GetMessageByIdAsync(int id)
        {
            var message = await _dbContext.Messages
                .Include(x => x.Sender).ThenInclude(x => x.Photos)
                .Include(x => x.Recipient).ThenInclude(x => x.Photos)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            return message;
        }
    }
}
