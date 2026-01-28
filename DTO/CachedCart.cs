namespace WallsShop.DTO;

public class CachedCart
{
    public ShoppingCart Cart { get; set; } = new ShoppingCart();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}