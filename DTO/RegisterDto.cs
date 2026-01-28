namespace WallsShop.DTO;

using System.ComponentModel.DataAnnotations;

public class RegisterDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$", 
        ErrorMessage = "Password must have uppercase, lowercase and a number")]
    public string Password { get; set; }

    public string PhoneNumber { get; set; }
    [Required]
    public string DisplayName { get; set; }
}