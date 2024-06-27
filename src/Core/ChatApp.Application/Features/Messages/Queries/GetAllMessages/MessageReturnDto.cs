namespace ChatApp.Application.Features.Messages.Queries.GetAllMessages
{
    public class MessageReturnDto
    {
        public string Content { get; set; } = string.Empty;
        public DateTime? DateRead { get; set; }
        public string SenderUserName { get; set; } = string.Empty;
        public string RecipientUserName { get; set; } = string.Empty;
    }
}
