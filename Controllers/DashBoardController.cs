using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WallsShop.Context;
using WallsShop.DTO;
using WallsShop.DTO.Dashboard;
using WallsShop.Entity;
using WallsShop.Repository;
using WallsShop.Repository.Dashboard;
using WallsShop.Services;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace WallsShop.Controllers.Dashboard;


[ApiController]
[Route("api/dashboard/[controller]")]
[Authorize(Roles = "Admin")]
public class DashBoardController : ControllerBase
{
    private readonly ILogger<DashBoardController> _logger;
    private readonly WallShopContext _context;
    private readonly ProductRepository producrrepo;
    private readonly CartService _cartService;
    private readonly WishlistRepository _wishlistService;
    private readonly UserManager<User> _userManager;
    private readonly DashboardCategoryRepository _categoryRepository;
    private readonly DashboardOfferRepository _offerRepository;
    private readonly OrderRepository _orderRepository;
    private readonly WishlistRepository _wishlistRepository;
    private  readonly FormRepository _formRepository;
    private readonly ProductRepository _productRepo;
    private readonly IWebHostEnvironment _env;
    private readonly IConfiguration _config;
    public DashBoardController(
        DashboardCategoryRepository Categoryrepository,
        DashboardOfferRepository offerRepository, 
        ProductRepository products,
        WallShopContext context, 
        CartService cartService,
        WishlistRepository wishlistService,
        UserManager<User> userManager,
        OrderRepository orderRepository,
        WishlistRepository wishlistRepository,
        FormRepository formRepository,
       ProductRepository productRepo,

      IWebHostEnvironment env,
        IConfiguration config,

        ILogger<DashBoardController> logger)
    {
        _logger = logger;
        _context = context;
        producrrepo = products;
        _cartService = cartService;
        _wishlistService = wishlistService;
        _userManager = userManager;
        _categoryRepository = Categoryrepository;
        _offerRepository = offerRepository;
        _productRepo = productRepo;
        _env = env;
        _config = config;
    }
    public class DashboardSummaryDto
    {
        public int TotalProducts { get; set; }
        public int TotalCategories { get; set; }
        public int TotalOffers { get; set; }
        public int TotalUsers { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalSales { get; set; }
    }
    [HttpGet("summary")]
    public async Task<IActionResult> GetDashboardSummary()
    {
        try
        {
            var totalProducts = await producrrepo.GetTotalProductsCount();
            var totalCategories = await _categoryRepository.GetTotalCategoriesCount();
            var totalOffers = await _offerRepository.GetTotalOffersCount();
            var totalUsers = await _userManager.Users.CountAsync();
            var totalOrders = await _context.Orders.CountAsync();
            var totalSales = await _context.Orders.SumAsync(o => o.TotalAmount);
            var summary = new DashboardSummaryDto
            {
                TotalProducts = totalProducts,
                TotalCategories = totalCategories,
                TotalOffers = totalOffers,
                TotalUsers = totalUsers,
                TotalOrders = totalOrders,
                TotalSales = totalSales

            };
            return Ok(new { success = true, data = summary });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching dashboard summary");
            return StatusCode(500, new { success = false, message = "حدث خطأ أثناء جلب ملخص لوحة التحكم" });
        }
    }
    class ProductsToMangeDto
    {
        public int TotalProducts { get; set; }
       
        public List<DashboardProductDto> Products { get; set; }
    }

    /// <summary>
    /// POST: api/product/add-product
    /// إضافة منتج - كل شيء JSON
    /// </summary>
    [HttpPost("add-product")]
  //  [Authorize(Roles = "Admin,SystemAdmin")]
    public async Task<IActionResult> AddProduct([FromForm] ProductAddDtoaren model)
    {
       
        // model.ImageFiles = ImageFiles;
        if (!ModelState.IsValid)
            return BadRequest(new { response = "بيانات غير صحيحة", errors = ModelState });

        var result = await _productRepo.AddProduct(model, _env, _config);

        if (result)
            return Ok(new { response = "تم إضافة المنتج بنجاح" });

        return BadRequest(new { response = "فشل في إضافة المنتج" });
    }
    [HttpDelete("delete-product")]
  //  [Authorize(Roles = "Admin,SystemAdmin")]
    public async Task<IActionResult> DeleteProduct([FromQuery] int id)
    {
        if (id <= 0)
            return BadRequest(new { response = "معرف المنتج غير صحيح" });

        var (success, message) = await _productRepo.DeleteProduct(id, _env);

        if (success)
            return Ok(new { response = message });

        return BadRequest(new { response = message });
    }
    [HttpGet("GetAllCustomers")]
    public async Task<IActionResult> GetAllCustomers()
    {
        try
        {
            var users = await _userManager.Users.ToListAsync();
            if(users == null || users.Count == 0)
            {
                return NotFound(new { success = false, message = "لا يوجد عملاء" });
            }
            var customerDtos = users.Select(user => new CustomerDto
            {
                Id = user.Id,
                UserName = user.Name,
           
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            }).ToList();
            return Ok(new { success = true, data = customerDtos });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching customers");
            return StatusCode(500, new { success = false, message = "حدث خطأ أثناء جلب العملاء" });
        }
    }
    [HttpDelete("DeleteCustomer")]
    public async Task<IActionResult> DeleteCustomer([FromQuery] string id)
    {
        if (string.IsNullOrEmpty(id))
            return BadRequest(new { response = "معرف العميل غير صحيح" });
        var user =  await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound(new { response = "العميل غير موجود" });

        var result = await _userManager.DeleteAsync(user);

        if (result.Succeeded)
            return Ok(new { response = "تم حذف العميل بنجاح" });

        // Aggregate error messages if any
        var errorMessage = string.Join("; ", result.Errors.Select(e => e.Description));
        return BadRequest(new { response = errorMessage });
    }
    //[HttpGet("GetALlOffers")]
    //public async Task<IActionResult> GetALlOffers()
    //{
    //    try
    //    {
    //        var offers = await _offerRepository.GetAllOffersAsync();
    //        if(offers == null || offers.Count == 0)
    //        {
    //            return NotFound(new { success = false, message = "لا يوجد عروض" });
    //        }
    //        return Ok(new { success = true, data = offers });
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, "Error fetching offers");
    //        return StatusCode(500, new { success = false, message = "حدث خطأ أثناء جلب العروض" });
    //    }
    //}

    [HttpPut("update-product")]
    //[Authorize(Roles = "Admin,SystemAdmin")]
    public async Task<IActionResult> UpdateProduct([FromQuery] int id, [FromForm] ProductUpdateDto model)
    {
        if (id <= 0)
            return BadRequest(new { response = "معرف المنتج غير صحيح" });

        if (!ModelState.IsValid)
            return BadRequest(new { response = "بيانات غير صحيحة", errors = ModelState });

        var result = await _productRepo.UpdateProduct(id, model, _env, _config);

        if (result)
            return Ok(new { response = "تم تعديل المنتج بنجاح" });

        return BadRequest(new { response = "فشل في تعديل المنتج أو المنتج غير موجود" });
    }
}
