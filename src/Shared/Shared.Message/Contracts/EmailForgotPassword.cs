namespace Shared.Message.Contracts;

public class EmailForgotPassword
{
    public string EmailTo { get; set; }
    public string HtmlTemplate { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public string UserName { get; set; }
}