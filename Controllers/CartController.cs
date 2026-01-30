using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WallsShop.DTO;
using WallsShop.Services;

namespace WallsShop.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly CartService _cartService;

    public CartController(CartService cartService) => _cartService = cartService;
    
    [Authorize]
    [HttpPost("add-item")]
    public async   Task<IActionResult> AddToCart([FromBody] CartItem itemDto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        await _cartService.AddToCart(userId, itemDto);
       
        return Ok(new { Message = "Item added to cart successfully" });
    }

    [HttpGet("get-cart")]
    public async Task<IActionResult> GetCartItems( [FromQuery] GetCartDto getCartDto, [FromQuery] string languageCode = "ar")
    {
        var authenticatedUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(authenticatedUserId))
        {
            return Ok(new {cart =  await _cartService.GetProductImageName(getCartDto.ShoppingCart.Items, languageCode)});
        }
        else
        {
            var items = new ShoppingCart()
            {
                Items = await _cartService.GetCartItems(authenticatedUserId, languageCode),
                UserId = getCartDto.UserId
            };
            return Ok(new {cart =items});
        }
    }
}