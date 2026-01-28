namespace WallsShop.DTO;

public class ShoppingCart
{
    public string UserId { get; set; } = string.Empty;
    public List<CartItem> Items { get; set; } = new List<CartItem>();
}