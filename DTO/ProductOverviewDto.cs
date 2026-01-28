namespace WallsShop.DTO;

public class ProductOverviewDto
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public decimal Price { get; set; }
    
    public decimal PriceAfterDiscount { get; set; }
    
    public decimal TotalRatingPeople { get; set; }
    
    public decimal AverageRatingPeople { get; set; }
    
    public string ImageUrl { get; set; }
    
    public bool IsInWishList { get; set; }
}