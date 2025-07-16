namespace Core.Entities;

public class UserProfile:BaseEntity
{
    public string ApplicationUserId { get; set; }
    public ApplicationUser? ApplicationUser { get; set; }
    public string? ProfilePicture { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
}
