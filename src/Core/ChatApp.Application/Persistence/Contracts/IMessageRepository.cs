using ChatApp.Application.Features.Messages.Query.GetMessageForUser;
using ChatApp.Application.Helpers;
using ChatApp.Core.Entities;

namespace ChatApp.Application.Persistence.Contracts
{
    public interface IMessageRepository : IGenericRepository<Message>
    {
        Task<PagedList<MessageDto>> GetMessagesForUserAsync(MessageParams messageParams);
        Task<IEnumerable<MessageDto>> GetMessagesIsReadAsync(string currentUserName, string recipientUserName);

    }
}
