namespace WallsShop.DTO;

public class GetCartDto
{
    public string UserId { get; set; } = string.Empty;
    public ShoppingCart ShoppingCart { get; set; }
}