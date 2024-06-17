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
                "Inbox" => query.Where(x => x.Recipient.UserName == messageParams.UserName),
                "Outbox" => query.Where(x => x.Sender.UserName == messageParams.UserName),
                _ => query.Where(x => x.Recipient.UserName == messageParams.UserName && x.DateRead == null),
            };

            var messageQuery = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);

            return await PagedList<MessageDto>.CreateAsync(messageQuery, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDto>> GetMessagesIsReadAsync(string currentUserName, string recipientUserName)
        {
            var messages = _dbContext.Messages
                .Include(x => x.Sender).ThenInclude(x => x.Photos)
                .Include(x => x.Recipient).ThenInclude(x => x.Photos)
                .AsNoTracking()
                .Where(x => x.Recipient.UserName == currentUserName
                    && x.Sender.UserName == recipientUserName || x.Recipient.UserName == recipientUserName
                    && x.Sender.UserName == currentUserName)
                .OrderByDescending(x => x.MessageSend)
                .ProjectTo<MessageDto>(_mapper.ConfigurationProvider)
                .ToList();

            var unReadMessages = _dbContext.Messages
                .Where(x => x.DateRead == null && x.Recipient.UserName == currentUserName)
                .ToList();

            if (unReadMessages.Any())
            {
                foreach (var item in unReadMessages)
                {
                    item.DateRead = DateTime.Now;
                    _dbContext.Messages.Update(item);
                    _dbContext.SaveChanges();
                }
            }

            return _mapper.Map<IEnumerable<MessageDto>>(messages);
        }
    }
}
