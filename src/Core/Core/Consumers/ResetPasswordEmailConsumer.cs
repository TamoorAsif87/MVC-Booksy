namespace Core.Consumers;

public class ResetPasswordEmailConsumer : IConsumer<EmailForgotPassword>
{
    private readonly IEmailService _emailService;

    public ResetPasswordEmailConsumer(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task Consume(ConsumeContext<EmailForgotPassword> context)
    {
        await _emailService.SendForgotPasswordEmail(
            context.Message.EmailTo,
            context.Message.HtmlTemplate,
            context.Message.Subject,
            context.Message.Body,
            context.Message.UserName
        );
    }
}
