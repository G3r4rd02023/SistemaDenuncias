using UMA_SYSTEM.Frontend.Models;

namespace UMA_SYSTEM.Frontend.Services
{
    public interface IMailService
    {
        Response SendMail(string toName, string toEmail, string subject, string body);
    }
}
