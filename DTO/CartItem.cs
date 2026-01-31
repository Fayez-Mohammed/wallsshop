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
    public decimal OriginalPrice { get; set; } 
    public int Quantity { get; set; } 
    
    public string ImageUrl { get; set; } = string.Empty;
}
public class CartResponseDto
{
    public List<CartItem> Items { get; set; } = new();
    public OrderSummaryDto Summary { get; set; } = new();
}

public class OrderSummaryDto
{
    public decimal TotalOriginalPrice { get; set; } // السعر قبل الخصم
    public decimal TotalDiscount { get; set; }      // قيمة التوفير (Savings)
    public decimal TotalPrice { get; set; }         // السعر النهائي المطلوب دفعه
    public int TotalProductsCount { get; set; }     // عدد المنتجات (الكميات)
}


public class UpdateQuantityRequestDto
{
    public int ProductId { get; set; }
    public int VarianceId { get; set; } = 0;
    public int Quantity { get; set; }
    public string? Color { get; set; } // أو int لو كنت بتستخدم Enum/ID
}

