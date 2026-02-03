using Microsoft.AspNetCore.Authentication.JwtBearer; // Add this
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens; // Add this
using Microsoft.OpenApi.Models;
using System.Text;
using WallsShop.Context;
using WallsShop.Entity;
using WallsShop.Repository;
using WallsShop.Repository.Dashboard;
using WallsShop.Services;
//using Swashbuckle.AspNetCore.SwaggerGen;
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
builder.Services.AddScoped<DashboardCategoryRepository>();
builder.Services.AddScoped<DashboardOfferRepository>();
builder.Services.AddScoped<OrderRepository>();
builder.Services.AddSignalR();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
////////////////
//builder.Services.AddDistributedMemoryCache();

//builder.Services.AddScoped<CartService>();
////////////////////
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});
//////////Swagger\\\\\\\\\\\\\\\\\\\\
///

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "WallsShop API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement()
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

//////////////////////////
var app = builder.Build();

// --- 2. Middleware Pipeline ---
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "WallsShop API v1");
    options.RoutePrefix = string.Empty;
}
);
//if (app.Environment.IsDevelopment())
//{
//    app.UseDeveloperExceptionPage();

    //    app.UseSwagger();
    //    app.UseSwaggerUI();
    //}
//else
//{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
//}

app.UseCors("AllowAll");

app.UseHttpsRedirection();
app.UseStaticFiles(); 
app.UseRouting();

app.UseAuthentication(); 
app.UseAuthorization(); 

app.MapControllers();
app.MapHub<ProductViewHub>("/hubs/product-views");
app.Run();