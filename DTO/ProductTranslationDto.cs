using System.Drawing;
using WallsShop.Entity;

namespace WallsShop.DTO;

public class ProductTranslationDto
{
    public int Id { get; set; }
    
    public int ProductId { get; set; }
    
    public string Category { get; set; }
    
    public string SKU { get; set; }
    
    public decimal AverageRate { get; set; }
    
    public string Name { get; set; }
    
    public decimal Price { get; set; }
    
    public decimal PriceAfterDiscount { get; set; }
    
    public string ShortDescription { get; set; }
 

    public List<string> Descriptions { get; set; }
    public List<Variant> Variants { get; set; }
    
    public List<ProductImageDto> Images { get; set; }
    
    public List<string> Colors { get; set; }
    
    public bool IsInWishList { get; set; }
    public int PageNumber { get; internal set; }
}