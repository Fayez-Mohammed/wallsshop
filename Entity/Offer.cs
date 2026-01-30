namespace WallsShop.Entity;

public class Offer
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public string ArabicName { get; set; }
    
    public string Description { get; set; }
    
    public string ArabicDescription { get; set; }
    
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    
    public string CateogryValue { get; set; }
    ////////////
    public string? ImageUrl { get; set; }

    public bool IsActive { get; set; } = true;


    public bool IsRunning => IsActive && EndDate >= DateOnly.FromDateTime(DateTime.Now);
}
