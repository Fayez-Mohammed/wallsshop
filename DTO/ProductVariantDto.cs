namespace WallsShop.DTO;

public class ProductVariantDto
{
    public int Id { get; set; }
    public string Type { get; set; }
    public string Size { get; set; }
    
    public decimal Price { get; set; }
    
    public decimal PriceBeforeDiscount { get; set; }
    public string ArabicPrice { get; set; }
    
    public string ArabicPriceAfterDiscount { get; set; }
}
public class ProductVariantENARDto
{
    public int Id { get; set; }
    public string Type { get; set; }
    public string Size { get; set; }
    
    public decimal Price { get; set; }
    
  public decimal PriceBeforeDiscount { get; set; }
   // public string ArabicPrice { get; set; }
    
 //   public string ArabicPriceAfterDiscount { get; set; }
    public string ArabicType { get;  set; }
    public string ArabicSize { get;  set; }
}