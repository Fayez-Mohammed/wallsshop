using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Security.Claims;
using System.Text.Json;
using WallsShop.DTO;
using WallsShop.Services;

namespace WallsShop.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly CartService _cartService;

    public CartController(CartService cartService) => _cartService = cartService;
    
    [Authorize]
    [HttpPost("add-item")]
    public async   Task<IActionResult> AddToCart([FromBody] CartItem itemDto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        await _cartService.AddToCart(userId, itemDto);
       
        return Ok(new { Message = "Item added to cart successfully" });
    }

    //[HttpPost("get-cart")]
    //public async Task<IActionResult> GetCartItems([FromBody] GetCartDto getCartDto, [FromQuery] string languageCode = "ar")
    //{
    //    var authenticatedUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    //    List<CartItem> finalItems = new List<CartItem>();
    //    if (string.IsNullOrEmpty(authenticatedUserId))
    //    {
    //        if (getCartDto?.ShoppingCart?.Items == null || !getCartDto.ShoppingCart.Items.Any())
    //        {
    //            return Ok(new { cart = new List<CartItem>() });
    //        }
    //        return Ok(new {cart =  await _cartService.GetProductImageName(getCartDto.ShoppingCart.Items, languageCode)});
    //    }
    //    else
    //    {
    //        finalItems = await _cartService.GetCartItems(authenticatedUserId, languageCode);
    //        //var items = new ShoppingCart()
    //        //{
    //        //    Items = await _cartService.GetCartItems(authenticatedUserId, languageCode),
    //        //    UserId = getCartDto.UserId
    //        //};
    //        //return Ok(new {cart =items});
    //    }
    //    var summary = new OrderSummaryDto
    //    {
    //        TotalOriginalPrice = finalItems.Sum(i => i.OriginalPrice * i.Quantity),

    //        TotalPrice = finalItems.Sum(i => i.UnitPrice * i.Quantity),

    //        TotalProductsCount = finalItems.Sum(i => i.Quantity)
    //    };


    //    summary.TotalDiscount = summary.TotalPrice - summary.TotalOriginalPrice;

    //    return Ok(new CartResponseDto
    //    {
    //        Items = finalItems,
    //        Summary = summary
    //    });
    //}

    [HttpPost("get-cart")]
    public async Task<IActionResult> GetCartItems([FromBody] GetCartDto getCartDto, [FromQuery] string languageCode = "ar")
    {
        var authenticatedUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        List<CartItem> finalItems = new List<CartItem>();

        if (string.IsNullOrEmpty(authenticatedUserId))
        {
          
            if (getCartDto?.ShoppingCart?.Items != null && getCartDto.ShoppingCart.Items.Any())
            {
                finalItems = await _cartService.GetProductImageName(getCartDto.ShoppingCart.Items, languageCode);
            }
        }
        else
        {
            finalItems = await _cartService.GetCartItems(authenticatedUserId, languageCode);
        }

      
        var summary = new OrderSummaryDto
        {
            TotalOriginalPrice = finalItems.Sum(i => i.OriginalPrice * i.Quantity),
            TotalPrice = finalItems.Sum(i => i.UnitPrice * i.Quantity),
            TotalProductsCount = finalItems.Sum(i => i.Quantity)
        };

        summary.TotalDiscount = summary.TotalPrice - summary.TotalOriginalPrice;

        return Ok(new CartResponseDto
        {
            Items = finalItems,
            Summary = summary
        });
    }

    // داخل CartController.cs

    [HttpPost("update-quantity")]
    public async Task<IActionResult> UpdateQuantity([FromBody] UpdateQuantityRequestDto request, [FromQuery] string languageCode = "ar")
    {
        
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

     
        var updatedItems = await _cartService.UpdateQuantityAsync(
            userId,
            request.ProductId,
            request.VarianceId,
            request.Quantity,
            request.Color,
            languageCode
        );

        if (updatedItems == null)
        {
            return NotFound(new { Message = "Product not found in cart" });
        }

        var summary = new OrderSummaryDto
        {
            TotalOriginalPrice = updatedItems.Sum(i => i.OriginalPrice * i.Quantity),
            TotalPrice = updatedItems.Sum(i => i.UnitPrice * i.Quantity),
            TotalProductsCount = updatedItems.Sum(i => i.Quantity)
        };

        summary.TotalDiscount = summary.TotalPrice - summary.TotalOriginalPrice;

        return Ok(new CartResponseDto
        {
            Items = updatedItems,
            Summary = summary
        });
    }
    // داخل CartController.cs

    [HttpDelete("delete-item")]
    public async Task<IActionResult> DeleteItem([FromBody] UpdateQuantityRequestDto request, [FromQuery] string languageCode = "ar")
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

      
        var updatedItems = await _cartService.RemoveItemAsync(
            userId,
            request.ProductId,
            request.VarianceId,
            request.Color,
            languageCode
        );

        if (updatedItems == null)
        {
            return NotFound(new { Message = "Item not found in cart" });
        }

        var summary = new OrderSummaryDto
        {
            TotalOriginalPrice = updatedItems.Sum(i => i.OriginalPrice * i.Quantity),
            TotalPrice = updatedItems.Sum(i => i.UnitPrice * i.Quantity),
            TotalProductsCount = updatedItems.Sum(i => i.Quantity)
        };

        summary.TotalDiscount = summary.TotalPrice - summary.TotalOriginalPrice;

        return Ok(new CartResponseDto
        {
            Items = updatedItems,
            Summary = summary
        });
    }
}