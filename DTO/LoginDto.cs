using WallsShop.Entity;

namespace WallsShop.DTO;

using System.ComponentModel.DataAnnotations;

public class LoginDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
    
    public WhishlistDto Wishlist { get; set; }
    
    public ShoppingCart Item { get; set; }
}

 