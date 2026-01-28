using System.ComponentModel.DataAnnotations;

namespace WallsShop.DTO;
public class FormDto
{
    public string Name { get; set; }
    
    public string Email { get; set; }
    public string Address { get; set; }
    public string Message { get; set; }
    [Required]
    public string PhoneNumber { get; set; }
    
    public DateOnly Date { get; set; }
}