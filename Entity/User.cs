using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace WallsShop.Entity;

public class User : IdentityUser
{
    [MaxLength(200)]
    public string Name { get; set; } 
    
    
    public virtual List<Wishlist> Wishlist { get; set; }
 //   public virtual List<Wishlist> WishlistItems { get; set; }
}