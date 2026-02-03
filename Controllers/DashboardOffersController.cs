using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WallsShop.DTO.Dashboard;
using WallsShop.Repository.Dashboard;

namespace WallsShop.Controllers.Dashboard;


[ApiController]
[Route("api/dashboard/[controller]")]
[Authorize(Roles = "Admin")] 
[Authorize] 
public class DashboardOffersController : ControllerBase
{
    private readonly DashboardOfferRepository _repository;
    private readonly ILogger<DashboardOffersController> _logger;

    public DashboardOffersController(
        DashboardOfferRepository repository,
        ILogger<DashboardOffersController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

   
    [HttpGet("GetOffers")]
    public async Task<IActionResult> GetAllOffers()
    {
        try
        {
            var offers = await _repository.GetAllOffers();
            return Ok(new { success = true, data = offers });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching all offers");
            return StatusCode(500, new { success = false, message = "حدث خطأ أثناء جلب العروض" });
        }
    }

  
    [HttpGet("SpecificOffer")]
    public async Task<IActionResult> GetOfferById([FromQuery] int id)
    {
        try
        {
            var offer = await _repository.GetOfferById(id);
            
            if (offer == null)
                return NotFound(new { success = false, message = "العرض غير موجود" });

            return Ok(new { success = true, data = offer });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching offer {OfferId}", id);
            return StatusCode(500, new { success = false, message = "حدث خطأ أثناء جلب العرض" });
        }
    }

  
    [HttpPost]
    public async Task<IActionResult> CreateOffer([FromForm] CreateUpdateOfferDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "بيانات غير صحيحة", errors = ModelState });

            var (success, message, offerId) = await _repository.CreateOffer(dto);

            if (!success)
                return BadRequest(new { success = false, message });

            return CreatedAtAction(
                nameof(GetOfferById),
                new { id = offerId },
                new { success = true, message, offerId }
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating offer");
            return StatusCode(500, new { success = false, message = "حدث خطأ أثناء إنشاء العرض" });
        }
    }


    [HttpPut("UpdateOffer")]
    public async Task<IActionResult> UpdateOffer([FromQuery] int id, [FromForm] CreateUpdateOfferDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "بيانات غير صحيحة", errors = ModelState });

            var (success, message) = await _repository.UpdateOffer(id, dto);

            if (!success)
                return BadRequest(new { success = false, message });

            return Ok(new { success = true, message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating offer {OfferId}", id);
            return StatusCode(500, new { success = false, message = "حدث خطأ أثناء تحديث العرض" });
        }
    }

   
    [HttpDelete("DeleteOffer")]
    public async Task<IActionResult> DeleteOffer([FromQuery] int id)
    {
        try
        {
            var (success, message) = await _repository.DeleteOffer(id);

            if (!success)
                return BadRequest(new { success = false, message });

            return Ok(new { success = true, message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting offer {OfferId}", id);
            return StatusCode(500, new { success = false, message = "حدث خطأ أثناء حذف العرض" });
        }
    }
}
