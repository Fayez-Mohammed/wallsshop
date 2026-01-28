using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WallsShop.DTO;
using WallsShop.Entity;
using WallsShop.Repository;

namespace WallsShop.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReviewController(ReviewRepository repo,UserManager<User> _userManager ) : ControllerBase 
{
    [AllowAnonymous]
    [HttpGet("reviews")]
    public async Task<IActionResult> GetReviews([FromQuery] int productId)
    {
        var userName = await _userManager.GetUserAsync(User);
        bool isAdmin = User.IsInRole("Admin");
        var reviews = await repo.GetProductReviews(productId,userName?.Name ?? "",isAdmin);
        return Ok(new {response = reviews});
    }
    
    [HttpPost("create-review")]
    public async Task<IActionResult> CreateReview([FromBody] ReviewDto review)
    {
        
        var result = await repo.CreateReview( review , User , _userManager);
        if (!result)
            return BadRequest(new {message = "Could not create review"});
        return Ok(new {message = "Review created successfully"});
    }

    [HttpDelete("delete-review")]
    public async Task<IActionResult> DeleteReview([FromQuery] int id)
    {
        var result = await repo.DeleteReview(id);
        if (!result)
            return BadRequest(new { message = "Could not delete review" });
        return Ok(new { message = "Review deleted successfully" });
    }
}