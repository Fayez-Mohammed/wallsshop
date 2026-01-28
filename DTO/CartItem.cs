namespace WallsShop.DTO;

public class CartItem
{
    public int ProductId { get; set; }
    
    public int VariantId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    
    public string Type { get; set; } = string.Empty;
    
    public string Size { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; } 
    
    public string ImageUrl { get; set; } = string.Empty;
}