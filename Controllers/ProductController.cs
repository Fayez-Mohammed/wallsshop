using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WallsShop.DTO;
using WallsShop.Repository;
namespace WallsShop.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController(ProductRepository products , IWebHostEnvironment env) : ControllerBase
{    
     [HttpGet("product")]
     public async Task<IActionResult> GetProductById
          ([FromQuery] QueryParameters queryParameters)
     {
        string? userId = null;
        if(User.Identity != null && User.Identity.IsAuthenticated)
        {
            userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        }
          if (queryParameters.LanguageCode == "ar" || string.IsNullOrEmpty(queryParameters.LanguageCode))
          {
               var list = await products.GetArabicProductById(queryParameters,userId);

               return Ok(new { Response = list });
          }
          var list1 = await products.GetEnglishProductById(queryParameters,userId);
          
          return Ok(new {Response =list1});
     }
     
     [HttpGet("products")]
     public async Task<IActionResult> GetProducts
          ([FromQuery] QueryParameters queryParameters)
    {
        string? userId = null;
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        }
        if (queryParameters.LanguageCode == "ar" || string.IsNullOrEmpty(queryParameters.LanguageCode))
          {
               var list = await products.GetProductsOverview(queryParameters,userId);

               return Ok(new { Response = list });
          }
          var list1 = await products.GetProductsTranslationOverview(queryParameters,userId);

          return Ok(new {Response =list1});
     }
     
     [HttpGet("related-product")]
     public async Task<IActionResult> GetRelatedProducts
          ([FromQuery] QueryParameters queryParameters , [FromQuery] int id )
    {
        string? userId = null;
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        }
        if (queryParameters.LanguageCode == "ar" || string.IsNullOrEmpty(queryParameters.LanguageCode))
          {

               var list = await products.GetProductsByCategory(queryParameters, id,userId);

               list = list.Where(p => p.Id != id).ToList();

               return Ok(new { Response = list });
          }
          var list1 = await products.GetProductTranslationsByCategory(queryParameters, id,userId);
          
          list1 = list1.Where(p => p.Id != id).ToList();
          
          return Ok(new {Response =list1});
     }
     
     [HttpGet("top-recent-product")]
     public async Task<IActionResult> GetTopRecentProducts
          ([FromQuery] QueryParameters queryParameters)
    {
        string? userId = null;
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        }
        if (queryParameters.LanguageCode == "ar" || string.IsNullOrEmpty(queryParameters.LanguageCode))
          {

               var list = await products.GetTop4RecentProducts(queryParameters,userId);

               return Ok(new { Response = list });
          }
          var list1 = await products.GetTop4RecentProductTranslations(queryParameters,userId);

          return Ok(new {Response =list1});
     }

     [HttpGet("top-rated-product")]
     public async Task<IActionResult> GetTopRatedProducts
          ([FromQuery] QueryParameters queryParameters)
    {
        string? userId = null;
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        }
        if (queryParameters.LanguageCode == "ar" || string.IsNullOrEmpty(queryParameters.LanguageCode))
          {

               var list = await products.GetTop4RatedProducts(queryParameters,userId);

               return Ok(new { Response = list });
          }
          var list1 = await  products.GetTop4RatedProductTranslations(queryParameters,userId);
          
          return Ok(new {Response =list1});
     }
}