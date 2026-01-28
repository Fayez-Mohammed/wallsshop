using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WallsShop.Properties.Entity;

namespace WallsShop.Entity;
 
public sealed class Wishlist
{
    [Key]
    public int Id { get; set; }
    
    [Required]
  
    public   string UserId { get; set; }
    
    [Required]
    public required int ProductId { get; set; }
    
    
    [ForeignKey("ProductId")]
    public Product Product { get; set; }
}