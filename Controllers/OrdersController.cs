using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WallsShop.Context;
using WallsShop.DTO;
using WallsShop.Entity;
using WallsShop.Properties.Entity;
using WallsShop.Services;

namespace WallsShop.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly WallShopContext _context;
    private readonly CartService _cartService;

    public OrdersController(WallShopContext context, CartService cartService)
    {
        _context = context;
        _cartService = cartService;
    }

    [HttpPost("MakeOrder")]
    public async Task<IActionResult> MakeOrder([FromBody] CreateOrderDto dto)
    {
      
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

      
        var cartItems = await _cartService.GetCartItems(userId, "ar");

        if (cartItems == null || !cartItems.Any())
        {
            return BadRequest("Cart is empty");
        }

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            
            decimal totalAmount = cartItems.Sum(x => x.UnitPrice * x.Quantity);

            
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                Status = "Pending",
                TotalAmount = totalAmount,

               
                ReceiverName = dto.FullName,
                ReceiverPhone = dto.PhoneNumber,
                ShippingAddress = dto.City + " - " + dto.Address,

             
                OrderDetailsList = cartItems.Select(item => new OrderDetails
                {
                    ProductId = item.ProductId,

                    ProductName = item.ProductName,
                    Price = item.UnitPrice,

                    Quantity = item.Quantity,
                    VariantId = item.VariantId != 0 ? item.VariantId : null,
                    Color = item.Color
                }).ToList()
            };

    
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

     
            _cartService.ClearCart(userId);

        
            await transaction.CommitAsync();

            return Ok(new { Message = "Order created successfully", OrderId = order.Id });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return StatusCode(500, "Error creating order: " + ex.Message);
        }
    }
}