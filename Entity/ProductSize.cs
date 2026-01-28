using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WallsShop.Properties.Entity;

namespace WallsShop.Entity;

public class ProductSize
{
    [Key]
    public int Id { get; set; }
    
    [ForeignKey("Product")]
    public int ProductId { get; set; }
    
    public string Size { get; set; }
    
    public virtual Product Product { get; set; }
}