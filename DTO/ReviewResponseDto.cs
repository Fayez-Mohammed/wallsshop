namespace WallsShop.DTO;

public class ReviewResponseDto
{ 
    public int ProductId { get; set; }
    
    public decimal AverageRating { get; set; }
    
    public decimal TotalReviews { get; set; }

    public List<SingleReviewDto> SingleReviews { get; set; } = new List<SingleReviewDto>();

}