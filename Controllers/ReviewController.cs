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

public class ReviewController(ReviewRepository repo,UserManager<User> _userManager ) : ControllerBase 
{
    // [AllowAnonymous]
    //[HttpGet("reviews")]
    //public async Task<IActionResult> GetReviews([FromQuery] int productId)
    //{
    //    var userName = await _userManager.GetUserAsync(User);
    //    bool isAdmin = User.IsInRole("Admin");
    //    var reviews = await repo.GetProductReviews(productId, userName?.Name ?? "", isAdmin);

    //    return Ok(new { response = reviews });


    //}

   // [Authorize]
    
    [HttpGet("reviews")]
    public async Task<IActionResult> GetReviews([FromQuery] int productId)
    {
        var user = await _userManager.GetUserAsync(User);
        bool isAdmin = User.IsInRole("Admin");
        string userId = user?.Id ?? ""; // Changed from Name to UserName

        var reviews = await repo.GetProductReviews(productId, userId, isAdmin);
        return Ok(new { response = reviews });
    }
    [Authorize]
    [HttpPost("create-review")]
    public async Task<IActionResult> CreateReview([FromBody] ReviewDto review)
    {
        
        var result = await repo.CreateReview( review , User , _userManager);
        if (result==null)
            return BadRequest(new {message = "error while creating review"});

        return Ok(result);
    }
    [Authorize]
    [HttpDelete("delete-review")]
    public async Task<IActionResult> DeleteReview([FromQuery] int id)
    {
        var result = await repo.DeleteReview(id,User,_userManager);
        if (result==null)
            return BadRequest(new { message = "Some thing wrong happend" });
        return Ok(result);
    }
}