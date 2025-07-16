namespace Core.DTO;

public class RegisterDto
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

}
