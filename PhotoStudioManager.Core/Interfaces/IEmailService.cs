namespace PhotoStudioManager.Core.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
    Task SendSessionConfirmationAsync(string to, string sessionTitle, DateTime sessionDate);
    Task SendPaymentConfirmationAsync(string to, decimal amount, string sessionTitle);
}
