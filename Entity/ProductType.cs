using System.ComponentModel.DataAnnotations.Schema;
using WallsShop.Properties.Entity;

namespace WallsShop.Entity;

public class ProductType
{
    public int Id { get; set; }
    
    public string Type { get; set; }
    
    [ForeignKey("ProductId")]
    public int ProductId { get; set; }
    
    public virtual Product? Product { get; set; }
}