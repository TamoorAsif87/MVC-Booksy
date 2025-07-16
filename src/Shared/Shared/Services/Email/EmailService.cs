using MailKit.Net.Smtp;
using MimeKit;
using Shared.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Polly.Retry;
using Polly;


namespace Shared.Services.Email;

public class EmailService:IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;
    public string SmtpServer { get; private set; }
    public int SmtpPort { get; private set; }
    public string SenderEmail { get; private set; }
    public string SmtpPassword { get; private set; }
    private readonly AsyncRetryPolicy _retryPolicy;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        SmtpServer = _configuration["EmailSettings:SmtpServer"]!;
        SmtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]!);
        SenderEmail = _configuration["EmailSettings:SenderEmail"]!;
        SmtpPassword = _configuration["EmailSettings:Password"]!;
        _logger = logger;

        _retryPolicy = Policy
        .Handle<Exception>()
        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(2), (exception, timeSpan, retryCount, context) =>
        {
            _logger.LogError($"Retry {retryCount} after {timeSpan.TotalSeconds}s due to: {exception.Message}");
        });
    }

    public  async Task SendForgotPasswordEmail(string emailTo, string htmlTemplate, string subject, string body,  string userName)
    {
        await BuildMail(emailTo, htmlTemplate, subject, body, userName);
    }

    public async Task SendOrderDetails(string emailTo, string htmlTemplate, string subject, string body, string userName)
    {
        await BuildMail(emailTo, htmlTemplate, subject, body, userName);
    }

    public async Task SendOrderInvoice(string emailTo, string htmlTemplate, string subject, string body, string userName)
    {
        await BuildMail(emailTo,htmlTemplate, subject, body, userName);
    }


    // private helper methods
    private async Task BuildMail(string emailTo, string htmlTemplate, string subject, string body, string userName)
    {
        try
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("admin", SenderEmail));
            email.To.Add(new MailboxAddress(userName, emailTo));
            email.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = htmlTemplate,
                TextBody = body
            };

            email.Body = bodyBuilder.ToMessageBody();

            await SendMail(email);
        }
        catch (Exception ex)
        {

            _logger.LogError(ex, "Failed to send forgot password email to {EmailTo}", emailTo);

            throw new InvalidOperationException("Failed to send forgot password email.", ex);
        }
    }

    private async Task SendMail(MimeMessage? email)
    {
        using var smtp = new SmtpClient();
        await _retryPolicy.ExecuteAsync(async () =>
        {
            try
            {
                await smtp.ConnectAsync(SmtpServer, SmtpPort, false);
                await smtp.AuthenticateAsync(SenderEmail, SmtpPassword);
                await smtp.SendAsync(email);
            }
            finally
            {
                await smtp.DisconnectAsync(true);
            }
        });
    }

   
}
