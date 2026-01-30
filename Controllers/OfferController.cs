using Microsoft.AspNetCore.Mvc;
using WallsShop.DTO;
using WallsShop.Repository;

namespace WallsShop.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OfferController(OfferRepository repo) : ControllerBase 
{
   [HttpPost("add-offer")]
   public async Task<IActionResult> AddOffer([FromForm] AddOfferDto model)
   {
      var result = await repo.AddOffer( model);
       if (result)
           return Ok(new {response = "Offer was added successfully"});
       return BadRequest();
   }

   [HttpGet("get-offer")]
   public async Task<IActionResult> GetOffers([FromQuery]  string languageCode)
   {
       var offers = await repo.GetOffers(languageCode);

       return Ok(new {reponse = offers});
   }
   
   
   [HttpDelete("delete-offer")]
    public async Task<IActionResult> DeleteOffer([FromQuery] int offerId)
    {
        var result = await repo.DeleteOffer(offerId);
        if (result)
            return Ok(new {response = "Offer was deleted successfully"});
        return BadRequest();
    }
}