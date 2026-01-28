using System.ComponentModel.DataAnnotations;

namespace WallsShop.DTO.OTP_Dtos;

public class VerifyOtpRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "OTP must be 6 digits.")]
    public string Otp { get; set; }
}