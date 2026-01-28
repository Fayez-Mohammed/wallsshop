using System.ComponentModel.DataAnnotations;

namespace WallsShop.Entity;

public class Form
{
    [Key]
    public int Id { get; set; }
    
    [MaxLength(1000)]
    [Display(Name = "Address")]
    public string? UserAddress { get; set; }
    
    [MaxLength(50)]
    [Display(Name = "Phone-Number")]
    public string? PhoneNumber { get; set; }
     
    
    [Required]
    [MaxLength(5000)]
    public required string Message { get; set; }
    
    [Display(Name = "Order Date")]
    public required DateOnly Date { get; set; }
}