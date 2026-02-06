using System.Security.Claims;
using System.Text;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using WallsShop.Context;
using WallsShop.DTO;
using WallsShop.DTO.OTP_Dtos;
using WallsShop.Entity;
using WallsShop.Repository;
using WallsShop.Services;
using ResetPasswordRequest = WallsShop.DTO.ResetPasswordRequest;
[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly WallShopContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IEmailService _emailService;
    private readonly CartService _cartService;
    private readonly WishlistRepository _wishlistService;
    private readonly ILogger<AccountController> _logger;

	public AccountController(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        ITokenService tokenService, WallShopContext context
        , IPasswordHasher<User> passwordHasher, IEmailService emailService
        , CartService cartService, WishlistRepository wishlistService
        , ILogger<AccountController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _context = context;
        _passwordHasher = passwordHasher;
        _emailService = emailService;
        _cartService = cartService;
        _wishlistService = wishlistService;
        _logger = logger;
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto model)
    {
        var user = new User { UserName = model.Email, Email = model.Email ,Name = model.DisplayName, PhoneNumber = model.PhoneNumber};
        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded) return BadRequest(new {response = result.Errors});

        
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

       
        var frontendUrl = "https://wallsshop.com/confirm-email";
        var callbackUrl = $"{frontendUrl}?token={encodedToken}&email={user.Email}";

        await _emailService.SendEmailAsync(user.Email, "Confirm your email",
            $"Please confirm your account by <a href='{callbackUrl}'>clicking here</a>.");
      //  var maxUserNumber = await _context.Users.MaxAsync(u => (int?)u.UserNumber) ?? 0;
    //    user.UserNumber = maxUserNumber + 1;
        //await _context.SaveChangesAsync();
        return Ok(new {response = "Registration successful. Please check your email to verify your account."});
    }
    
    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailDto model)
    {
        if (string.IsNullOrWhiteSpace(model.Token) || string.IsNullOrWhiteSpace(model.Email))
            return BadRequest(new {response ="Invalid parameters."});

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null) return NotFound(new {response = "User not found."});

        try 
        {
            string processedToken = model.Token.Replace(" ", "+");
            byte[] decodedBytes = WebEncoders.Base64UrlDecode(processedToken);
            string actualToken = Encoding.UTF8.GetString(decodedBytes);

            
            var result = await _userManager.ConfirmEmailAsync(user, actualToken);

            await _userManager.AddToRoleAsync(user,"User");

            if (result.Succeeded)
            { 
                return Ok(new {response = "Email verified!"});
            }

            return BadRequest(result.Errors);
        }
        catch (Exception ex)
        {
            return BadRequest(new {response = "The token format is invalid. Ensure the link wasn't modified."});
        }
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            
            if (user == null) return Unauthorized(new {response = "Invalid credentials"});
            if (user.IsBlocked)
            {
                return Unauthorized(new { response = "Your account has been blocked. Please contact support for assistance." });
            }
            var passwordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!passwordValid) return Unauthorized( new {response = "Invalid credentials"});
 
            if (loginDto.Item?.Items != null)
            {
                foreach (var item in loginDto.Item.Items)
                    await _cartService.AddToCart(user.Id, item);
            }
 
            if (loginDto.Wishlist?.ProductIds?.Any() == true)
                await _wishlistService.AddToWishlist(loginDto.Wishlist, user.Id);
         if(user.EmailConfirmed == false)
            {
                return Unauthorized(new { response = "Email not confirmed. Please verify your email before logging in." });
            }
            var token = _tokenService.CreateToken(user);

            return Ok(new { Email = user.Email, Token = token });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during login for email: {Email}", loginDto.Email);
			 
			try
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "login_error.txt");
                System.IO.File.WriteAllText(path, ex.ToString());
            }
            catch {   }

            throw;
        }
    }


        
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok(new { response  = "Logged out successfully" });
    }

    [Authorize]
    [HttpGet("user-info")]
    public async Task<IActionResult> GetCurrentUser()
    {
        
        var email = User.FindFirstValue(ClaimTypes.Email); 
       
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var userById = await _userManager.FindByIdAsync(userId);

        if (userById == null)
            return BadRequest();
        
        var Roles = await _userManager.GetRolesAsync(userById);


        var role = Roles.FirstOrDefault();
        if (email == null) return Unauthorized();

        var user = await _userManager.FindByEmailAsync(email);
    
        return Ok(new {
            user.Name,
            user.Email,
            user.PhoneNumber,
            role 
        });
    }
    
    [Authorize]
    [HttpPut("update-user-info")]
    public async Task<IActionResult> UpdateUserInfo([FromBody] UpdateUserDto updateUserDto)
    {
        var user = await _userManager.FindByEmailAsync(updateUserDto.Email);
        if (user == null) return NotFound(new {response ="User not found"});

        user.Name = updateUserDto.Name ?? user.Name;
        user.PhoneNumber = updateUserDto.PhoneNumber ?? user.PhoneNumber;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded) return BadRequest(new {response = result.Errors});

        return Ok(new { response  = "User information updated successfully" });
    }   
 
    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest model)
    {
        
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await _context.Users.FindAsync(userId);

        if (user == null) return Unauthorized();

       
        var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.OldPassword);
        if (verificationResult == PasswordVerificationResult.Failed)
        {
            return BadRequest(new {response = "The old password you entered is incorrect."});
        }

        
        user.PasswordHash = _passwordHasher.HashPassword(user, model.NewPassword);
        await _context.SaveChangesAsync();

        return Ok(new {response = "Password updated successfully."});
    }
    
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgetPasswordDto model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null) return Ok(new {response  = "If your email exists, an OTP has been sent."});

       
        var otp = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");

        await _emailService.SendEmailAsync(user.Email, "Reset Password OTP",
            $"Your password reset code is: {otp}. It expires in 15 minutes.");

        return Ok(new { response  = "OTP sent to email.", Email = model.Email });
    }
    
    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null) return BadRequest(new {response = "Invalid request."});

        var isValid = await _userManager.VerifyTwoFactorTokenAsync(user, "Email", model.Otp);

        if (!isValid) return BadRequest(new { response = "Invalid or expired OTP."});

         
        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
    
        return Ok(new { 
            Message = "OTP Verified.", 
            ResetToken = resetToken  
        });
    }
    [HttpPost("resend-otp")]
    public async Task<IActionResult> ResendOtp([FromBody] ResendOtpRequest model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null) return Ok(new {response = "OTP resent if account exists."});

        var otp = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
    
        await _emailService.SendEmailAsync(user.Email, "New OTP Code", 
            $"Your new code is: {otp}");

        return Ok(new {response  = "A new OTP has been sent."});
    }
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null) return BadRequest(new {response = "Invalid Request"});

         
        var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);

        if (result.Succeeded)
        {
            return Ok(new {response = "Your password has been reset successfully."});
        }

        return BadRequest(result.Errors);
    }
  
}