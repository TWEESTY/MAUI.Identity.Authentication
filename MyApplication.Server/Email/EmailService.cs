using MyApplication.Server.Email.Interfaces;

namespace MyApplication.Server.Email;

public class EmailService(ILogger<EmailService> logger) : IEmailService
{
    public Task SendConfirmationLinkForRegisteringAsync(string email, string confirmationLink)
    {
        logger.LogInformation("Confirmation link {ConfirmationLink}", confirmationLink);
        return Task.CompletedTask;
    }
}
