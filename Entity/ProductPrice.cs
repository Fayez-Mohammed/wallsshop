using System.ComponentModel.DataAnnotations.Schema;
using WallsShop.Properties.Entity;

namespace WallsShop.Entity;

public class ProductPrice
{
    public int Id { get; set; }
    
    [ForeignKey("ProductId")]
    public int ProductId { get; set; }
    
    public decimal Price { get; set; }
    
    public decimal PriceBeforeDiscount { get; set; }
    
    public decimal DiscountAmount { get; set; }
    
    public virtual  Product? Product { get; set; }
}