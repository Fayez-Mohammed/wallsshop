using Microsoft.AspNetCore.Mvc;
using WallsShop.DTO;
using WallsShop.Repository;

namespace WallsShop.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController(CategoryRepository repo) : ControllerBase
{
    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories([FromQuery] string LanguageCode )
    {
        return Ok(new {response = await repo.GetCategories(LanguageCode)});
    }
}