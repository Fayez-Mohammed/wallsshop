using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer; // Add this
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens; // Add this
using WallsShop.Context;
using WallsShop.Entity;
using WallsShop.Repository;
using WallsShop.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<WallShopContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services
    .AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<WallShopContext>()
    .AddDefaultTokenProviders();

// --- ADD THIS SECTION: JWT Configuration ---
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["TokenKey"]));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = key,
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});
// --------------------------------------------

builder.Services.AddSingleton<CartService>();
// 1. Load settings from appsettings.json
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddScoped<OfferRepository>();
builder.Services.AddScoped<WishlistRepository>();
builder.Services.AddTransient<IEmailService, SmtpEmailService>();
builder.Services.AddScoped<FormRepository>();
builder.Services.AddScoped<CategoryRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ReviewRepository>();
builder.Services.AddScoped<ProductRepository>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

// --- 2. Middleware Pipeline ---

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
  
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseCors("AllowAll");

app.UseHttpsRedirection();
app.UseStaticFiles(); 
app.UseRouting();

app.UseAuthentication(); 
app.UseAuthorization(); 

app.MapControllers();

app.Run();