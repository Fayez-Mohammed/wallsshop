namespace WallsShop.DTO;

public class ReviewDto
{
    public string Comment { get; set; }
    public int ProductId { get; set; }
    
    public decimal Rating { get; set; }
}