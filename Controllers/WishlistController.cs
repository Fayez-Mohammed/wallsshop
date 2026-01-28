using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WallsShop.DTO;
using WallsShop.Repository;

namespace WallsShop.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WishlistController(WishlistRepository repo , ProductRepository product ) : ControllerBase 
{
    [HttpPost("add")]
    public async  Task<IActionResult> Add([FromBody] WhishlistDto wishlistDto)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        await repo.AddToWishlist(wishlistDto, userId!);
        return Ok(new { Message = "Item added to wishlist successfully" });
    }
    [AllowAnonymous]
    [HttpGet("get")]
    public async Task<IActionResult> Get([FromBody] WhishlistDto wishlistDto)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (User.Identity.IsAuthenticated)
        {
            var response = await repo.GetWishlist(userId!);
            return Ok(new { response });
        }
        var products = await repo.GetProducts(wishlistDto);
        return Ok(new { products });
    }

    [HttpDelete("remove")]
    public async Task<IActionResult> Remove([FromQuery] int productId)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        await repo.RemoveFromWishlist(userId!, productId);
        return Ok(new { Message = "Item removed from wishlist successfully" });
    }

}