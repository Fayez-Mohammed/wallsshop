namespace WallsShop.DTO;

public class SingleReviewDto
{
    public string UserName { get; set; }
    
    public string Comment { get; set; }
    
    public DateTime Date { get; set; }
    
    public bool CanBeDeleted { get; set; }
    
    public decimal Rate { get; set; }
}