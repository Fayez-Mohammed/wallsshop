using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace WallsShop.Entity;

public class User : IdentityUser
{
    [MaxLength(200)]
    public string Name { get; set; } 
  //  public DateTime? DateOfCreation { get; set; }= DateTime.Now;
    // public int? UserNumber { get; set; }
    public bool IsBlocked { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public virtual List<Wishlist> Wishlist { get; set; }
 //   public virtual List<Wishlist> WishlistItems { get; set; }
}