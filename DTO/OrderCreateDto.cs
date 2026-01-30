using System.ComponentModel.DataAnnotations;

namespace WallsShop.DTO;

public class CreateOrderDto
{
    [Required]
    public string FullName { get; set; }

    [Required]
    public string Address { get; set; } // العنوان

    [Required]
    public string PhoneNumber { get; set; }

    public string? City { get; set; }
}