namespace WallsShop.Entity;

public class Coupon
{
    public int Id { get; set; }
    
    public string Code { get; set; }
    
    public decimal DiscountAmount { get; set; }
    
    public DateOnly ExpiryDate { get; set; }
}