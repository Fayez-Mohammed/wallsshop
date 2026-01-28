namespace WallsShop.DTO;

public class VariantDto
{
    public string SKU { get; set; }
    public string Size { get; set; }
    public decimal Price { get; set; }
    public decimal? PriceBeforeDiscount { get; set; }
}