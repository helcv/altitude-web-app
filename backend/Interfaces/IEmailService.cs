using backend.DTOs;

namespace backend.Interfaces
{
    public interface IEmailService
    {
        Task SendEmail (EmailMessageDto emailMessagDto);
    }
}
