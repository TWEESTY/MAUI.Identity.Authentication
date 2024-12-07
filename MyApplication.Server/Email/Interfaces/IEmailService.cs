
namespace MyApplication.Server.Email.Interfaces;
public interface IEmailService
{
    Task SendConfirmationLinkForRegisteringAsync(string email, string confirmationLink);
}
