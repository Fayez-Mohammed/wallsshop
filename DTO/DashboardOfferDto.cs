namespace WallsShop.DTO.Dashboard;


public class DashboardOfferDto
{
    public int Id { get; set; }
    
    public string TitleAR { get; set; }
    public string TitleEN { get; set; }
    
    public string DescriptionAR { get; set; }
    public string DescriptionEN { get; set; }
    
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    
    public bool IsActive { get; set; }
    
    public bool IsRunning { get; set; }
    public string CategoryAR { get; set; }
    public string CategoryEN { get; set; }
    public string CategoryValue { get; set; }
    
    public string? ImageUrl { get; set; }
}


public class CreateUpdateOfferDto
{
    public string TitleAR { get; set; }
    public string TitleEN { get; set; }
    
    public string DescriptionAR { get; set; }
    public string DescriptionEN { get; set; }
    
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public string CategoryValue { get; set; }
    
    public string? ImageUrlLink { get; set; }
    
    public IFormFile? ImageFile { get; set; }
}
