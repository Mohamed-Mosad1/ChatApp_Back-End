using ChatApp.Application.Features.Messages.Queries.GetMessageForUser;
using ChatApp.Application.Helpers;
using ChatApp.Core.Entities;
using ChatApp.Domain.Entities;

namespace ChatApp.Application.Persistence.Contracts
{
    public interface IMessageRepository : IGenericRepository<Message>
    {
        Task<PagedList<MessageDto>> GetMessagesForUserAsync(MessageParams messageParams);
        Task<IEnumerable<MessageDto>> GetMessagesIsReadAsync(string currentUserName, string recipientUserName);
        Task DeleteMessageAsync(Message message);
        Task<Message?> GetMessageByIdAsync(int id);

        void AddGroup(Group messageGroup);
        void RemoveConnection(Connection connection);
        void AddMessage(Message message);
        void RemoveMessage(Message message);
        Task<Connection?> GetConnectionAsync(string connectionId);
        Task<Group?> GetMessageGroupAsync(string groupName);
        Task<Group?> GetGroupForConnectionAsync(string connectionId);


    }
}
