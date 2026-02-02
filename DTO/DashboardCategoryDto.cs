namespace WallsShop.DTO.Dashboard;


public class DashboardCategoryDto
{
    public int Id { get; set; }
    
    public string NameAR { get; set; }
    public string NameEN { get; set; }
    
    public string CategoryValue { get; set; }
    
    public string ImageUrl { get; set; }
}


public class CreateUpdateCategoryDto
{
    public string NameAR { get; set; }
    public string NameEN { get; set; }
    
    //public string CategoryValue { get; set; }
    
    public string? ImageUrlLink { get; set; }
    
    public IFormFile? ImageFile { get; set; }
}
