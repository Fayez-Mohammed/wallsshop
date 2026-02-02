namespace WallsShop.DTO;

public class ProductAddDto
{
    public string Name { get; set; }
    //public string NameEN { get; set; }
    public string SKU { get; set; }
    public decimal Price { get; set; }
    public decimal PriceBeforeDiscount { get; set; }
    public decimal Discount { get; set; }
    public List<string> Size { get; set; }
    
    public List<string> Type { get; set; }
    public string CategoryValue { get; set; }
    public string LanguageCode { get; set; }
    public string Description { get; set; }
    //public string DescriptionEN { get; set; }
    
    public List<string> Colors { get; set; }
}