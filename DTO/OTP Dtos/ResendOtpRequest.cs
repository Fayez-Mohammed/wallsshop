using System.ComponentModel.DataAnnotations;

namespace WallsShop.DTO.OTP_Dtos;

public class ResendOtpRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}