using System.ComponentModel.DataAnnotations;

namespace WallsShop.DTO;

public class ChangePasswordRequest
{
    [Required]
    public string OldPassword { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$", 
        ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, and one number.")]
    public string NewPassword { get; set; }

    [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
    public string ConfirmNewPassword { get; set; }
}