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
    }
}
