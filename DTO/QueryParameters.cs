namespace WallsShop.DTO;

public class QueryParameters
{
    public string search { get; set; } = string.Empty;
    public string order { get; set; } = string.Empty;
    public string category { get; set; } = string.Empty;
    public int page { get; set; } = 0;
    public int pageSize { get; set; } = 10;
    
    public int id { get; set; }
    
    public string LanguageCode { get; set; } 
    
    public List<int> Ids { get; set; } = new List<int>(); // whishlist 
}