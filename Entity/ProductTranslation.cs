using System.ComponentModel.DataAnnotations.Schema;
using WallsShop.Properties.Entity;

namespace WallsShop.Entity;

public class ProductTranslation
{
    public int Id { get; set; }
    
    [ForeignKey(("ProductId"))]
    
    public int ProductId { get; set; }
    
    public string Category { get; set; }
    
    public string SKU { get; set; }
    
    public decimal AverageRate { get; set; }
    
    public string Name { get; set; }
    
    public decimal Price { get; set; }
    
    public decimal PriceAfterDiscount { get; set; }
    
    public string Description { get; set; }
    
    public virtual  Product Product { get; set; }
    
}