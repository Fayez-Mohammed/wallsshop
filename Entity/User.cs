using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace WallsShop.Entity;

public class User : IdentityUser
{
    [MaxLength(200)]
    public string Name { get; set; } 
    
    
    public virtual Wishlist Wishlist { get; set; }
}