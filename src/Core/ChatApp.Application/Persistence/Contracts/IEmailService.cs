using ChatApp.Domain.Entities;

namespace ChatApp.Application.Persistence.Contracts
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailModel emailModel);
    }
}
