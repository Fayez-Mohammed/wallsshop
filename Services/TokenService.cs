//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;
//using Microsoft.IdentityModel.Tokens;
//using WallsShop.Entity;

//namespace WallsShop.Services;

//public class TokenService : ITokenService
//{
//    private readonly IConfiguration _config;
//    private readonly SymmetricSecurityKey _key;

//    public TokenService(IConfiguration config)
//    {
//        _config = config;
//        // The key should be a long, complex string stored in appsettings.json
//        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["TokenKey"]));
//    }

//    public string CreateToken(User user)
//    {
//        // 1. Define the Claims (User data inside the token)
//        var claims = new List<Claim>
//        {
//            new Claim(JwtRegisteredClaimNames.NameId, user.Id),
//            new Claim(JwtRegisteredClaimNames.Email, user.Email),
//            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)
//        };

//        // 2. Create Signing Credentials
//        var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

//        // 3. Describe the Token (Expiration, Claims, etc.)
//        var tokenDescriptor = new SecurityTokenDescriptor
//        {
//            Subject = new ClaimsIdentity(claims),
//            Expires = DateTime.Now.AddDays(7), // Token valid for 7 days
//            SigningCredentials = creds
//        };

//        // 4. Create and Write the Token
//        var tokenHandler = new JwtSecurityTokenHandler();
//        var token = tokenHandler.CreateToken(tokenDescriptor);

//        return tokenHandler.WriteToken(token);
//    }
//}


//public interface ITokenService
//{
//    string CreateToken(User user);
//}

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using WallsShop.Entity;

namespace WallsShop.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;
    private readonly SymmetricSecurityKey _key;
    private readonly UserManager<User> _userManager;  

    public TokenService(IConfiguration config, UserManager<User> userManager)  
    {
        _config = config;
        _userManager = userManager;
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["TokenKey"]));
    }

    public async Task<string> CreateToken(User user)  
    {
        
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.NameId, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)
        };

        var roles = await _userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(7),
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}

public interface ITokenService
{
    Task<string> CreateToken(User user); 
}