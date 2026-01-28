using System.ComponentModel.DataAnnotations;

namespace WallsShop.DTO;

public class ResetPasswordRequest
{
    [Required]
    public string Token { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; } // It's safer to verify the email belongs to the token

    [Required]
    [StringLength(100, MinimumLength = 8)]
    public string NewPassword { get; set; }

    [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; }
}