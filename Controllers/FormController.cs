using Microsoft.AspNetCore.Mvc;
using WallsShop.DTO;
using WallsShop.Repository;

namespace WallsShop.Controllers;
[ApiController]
[Route("api/[controller]")]
public class FormController(FormRepository repo) : Controller
{
     [HttpPost("add-form")]
    public async Task<IActionResult> Create([FromBody] FormDto form )
    {
        try
        {
            var result = await repo.CreateForm(form);
            return Ok(result ? "Form submitted successfully." : "Failed to submit form.");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message); 
        }
    }
}