

namespace Core.Entities;

public class ApplicationUser:IdentityUser
{
    public required string Name { get; set; }

}
