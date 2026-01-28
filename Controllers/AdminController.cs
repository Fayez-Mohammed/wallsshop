using Microsoft.AspNetCore.Mvc;
using WallsShop.Repository;

namespace WallsShop.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController(OrderRepository repo) : Controller
{

    
}