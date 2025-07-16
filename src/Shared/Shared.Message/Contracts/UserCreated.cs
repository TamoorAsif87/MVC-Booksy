namespace Shared.Message.Contracts;

public class UserCreated
{
    public string UserId { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public string UserName { get; set; }
}