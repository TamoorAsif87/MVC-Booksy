namespace Shared.Contracts;

public interface IEmailService
{
    Task SendForgotPasswordEmail(string emailTo, string htmlTemplate, string subject, string body, string userName);

    Task SendOrderDetails(string emailTo, string htmlTemplate, string subject, string body, string userName);

    Task SendOrderInvoice(string emailTo, string htmlTemplate, string subject, string body, string userName);
}
