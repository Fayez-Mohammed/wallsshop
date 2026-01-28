using Microsoft.AspNetCore.Mvc;
using WallsShop.DTO;
using WallsShop.Repository;
namespace WallsShop.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController(ProductRepository products , IWebHostEnvironment env) : ControllerBase
{    
     [HttpGet("product")]
     public async Task<IActionResult> GetProductById
          ([FromBody] QueryParameters queryParameters)
     {
          if (queryParameters.LanguageCode == "ar" || string.IsNullOrEmpty(queryParameters.LanguageCode))
          {
               var list = await products.GetArabicProductById(queryParameters);

               return Ok(new { Response = list });
          }
          var list1 = await products.GetEnglishProductById(queryParameters);
          
          return Ok(new {Response =list1});
     }
     
     [HttpGet("products")]
     public async Task<IActionResult> GetProducts
          ([FromBody] QueryParameters queryParameters)
     {
          if (queryParameters.LanguageCode == "ar" || string.IsNullOrEmpty(queryParameters.LanguageCode))
          {
               var list = await products.GetProductsOverview(queryParameters);

               return Ok(new { Response = list });
          }
          var list1 = await products.GetProductsTranslationOverview(queryParameters);
          
          return Ok(new {Response =list1});
     }
     
     [HttpGet("related-product")]
     public async Task<IActionResult> GetRelatedProducts
          ([FromBody] QueryParameters queryParameters , [FromQuery] int id )
     {
          if (queryParameters.LanguageCode == "ar" || string.IsNullOrEmpty(queryParameters.LanguageCode))
          {

               var list = await products.GetProductsByCategory(queryParameters, id);

               list = list.Where(p => p.Id != id).ToList();

               return Ok(new { Response = list });
          }
          var list1 = await products.GetProductTranslationsByCategory(queryParameters, id);
          
          list1 = list1.Where(p => p.Id != id).ToList();
          
          return Ok(new {Response =list1});
     }
     
     [HttpGet("top-recent-product")]
     public async Task<IActionResult> GetTopRecentProducts
          ([FromBody] QueryParameters queryParameters)
     {
          if (queryParameters.LanguageCode == "ar" || string.IsNullOrEmpty(queryParameters.LanguageCode))
          {

               var list = await products.GetTop4RecentProducts(queryParameters);

               return Ok(new { Response = list });
          }
          var list1 = await products.GetTop4RecentProductTranslations(queryParameters);
          
          return Ok(new {Response =list1});
     }

     [HttpGet("top-rated-product")]
     public async Task<IActionResult> GetTopRatedProducts
          ([FromBody] QueryParameters queryParameters)
     {
          if (queryParameters.LanguageCode == "ar" || string.IsNullOrEmpty(queryParameters.LanguageCode))
          {

               var list = await products.GetTop4RatedProducts(queryParameters);

               return Ok(new { Response = list });
          }
          var list1 = await  products.GetTop4RatedProductTranslations(queryParameters);
          
          return Ok(new {Response =list1});
     }
}