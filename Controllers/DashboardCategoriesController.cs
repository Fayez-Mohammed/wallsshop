using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WallsShop.DTO.Dashboard;
using WallsShop.Repository.Dashboard;

namespace WallsShop.Controllers.Dashboard;


[ApiController]
[Route("api/dashboard/[controller]")]
[Authorize(Roles = "Admin")]
public class DashboardCategoriesController : ControllerBase
{
    private readonly DashboardCategoryRepository _repository;
    private readonly ILogger<DashboardCategoriesController> _logger;

    public DashboardCategoriesController(
        DashboardCategoryRepository repository,
        ILogger<DashboardCategoriesController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCategories()
    {
        try
        {
            var categories = await _repository.GetAllCategories();
            return Ok(new { success = true, data = categories });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching all categories");
            return StatusCode(500, new { success = false, message = "حدث خطأ أثناء جلب الأقسام" });
        }
    }


    [HttpGet("GetCategoryById")]
    public async Task<IActionResult> GetCategoryById([FromQuery] int id)
    {
        try
        {
            var category = await _repository.GetCategoryById(id);

            if (category == null)
                return NotFound(new { success = false, message = "القسم غير موجود" });

            return Ok(new { success = true, data = category });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching category {CategoryId}", id);
            return StatusCode(500, new { success = false, message = "حدث خطأ أثناء جلب القسم" });
        }
    }


    [HttpPost("CreateCategory")]
    public async Task<IActionResult> CreateCategory([FromForm] CreateUpdateCategoryDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "بيانات غير صحيحة", errors = ModelState });

            var (success, message, categoryId) = await _repository.CreateCategory(dto);

            if (!success)
                return BadRequest(new { success = false, message });

            return CreatedAtAction(
                nameof(GetCategoryById),
                new { id = categoryId },
                new { success = true, message, categoryId }
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating category");
            return StatusCode(500, new { success = false, message = "حدث خطأ أثناء إنشاء القسم" });
        }
    }


    [HttpPut("UpdateCategory")]
    public async Task<IActionResult> UpdateCategory([FromQuery] int id, [FromForm] CreateUpdateCategoryDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "بيانات غير صحيحة", errors = ModelState });

            var (success, message) = await _repository.UpdateCategory(id, dto);

            if (!success)
                return BadRequest(new { success = false, message });

            return Ok(new { success = true, message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating category {CategoryId}", id);
            return StatusCode(500, new { success = false, message = "حدث خطأ أثناء تحديث القسم" });
        }
    }


    [HttpDelete("DeleteCategory")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        try
        {
            var (success, message) = await _repository.DeleteCategory(id);

            if (!success)
                return BadRequest(new { success = false, message });

            return Ok(new { success = true, message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting category {CategoryId}", id);
            return StatusCode(500, new { success = false, message = "حدث خطأ أثناء حذف القسم" });
        }
    }
}
