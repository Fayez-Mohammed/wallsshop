using WallsShop.Entity;

namespace WallsShop.DTO;

public class ProductResponseDto
{
    public int PageNumber { get; set; }
    public int Id { get; set; }
    public string Name { get; set; }
    public string SKU { get; set; }
    public decimal Price { get; set; }
    
    public decimal PriceAfterDiscount { get; set; }
    public bool IsInWishList { get; set; }
   
    public List<string> Colors { get; set; }
    
    
    public string ShortDescription { get; set; }
    
    
    public string Description { get; set; }
    
    public string CateogryValue { get; set; }
    public string Category { get; set; }
    
     
    public List<ProductImageDto> Images { get; set; }
    public List<ProductVariantDto> Variants { get; set; }
}
public class PagedResult<T>
{
    public List<T> Data { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public string CategoryName { get; set; }
}