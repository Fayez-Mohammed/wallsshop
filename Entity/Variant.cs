using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WallsShop.Properties.Entity;

public class Variant
{
    [Key]
    public int Id { get; set; }

    public string SKU { get; set; } // REMOVED [ForeignKey] from here
    
    public int ProductId { get; set; } // The ID column
    
    [ForeignKey("ProductId")]  
    public virtual Product? Product { get; set; }
    
    public string Size { get; set; }
    public string Type { get; set; }
    public decimal Price { get; set; }
    public decimal? PriceBeforeDiscount { get; set; }
  
    public decimal DiscountRate { get; set; }
    public string LanguageCode { get; set; }
}