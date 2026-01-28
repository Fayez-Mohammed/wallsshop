namespace WallsShop.DTO;

public class ProductTranslationOverviewDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public decimal PriceAfterDiscount { get; set; }
    public string ImageUrl { get; set; }
    
    public decimal TotalPeopleRating { get; set; }
    
    public decimal AverageRatingPeople { get; set; }
    public bool IsInWishList { get; set; }
}