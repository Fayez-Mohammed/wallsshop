using Microsoft.EntityFrameworkCore;
using WallsShop.Context;
using WallsShop.DTO.Dashboard;
using WallsShop.Entity;

namespace WallsShop.Repository.Dashboard;

public class DashboardCategoryRepository
{
    private readonly WallShopContext _ctx;
    private readonly IWebHostEnvironment _env;
    private readonly IConfiguration _config;

    public DashboardCategoryRepository(
        WallShopContext ctx,
        IWebHostEnvironment env,
        IConfiguration config)
    {
        _ctx = ctx;
        _env = env;
        _config = config;
    }

    
    public async Task<List<DashboardCategoryDto>> GetAllCategories()
    {
        return await _ctx.CategoryImages
            .Select(c => new DashboardCategoryDto
            {
                Id = c.Id,
                NameAR = c.Category.Replace("-"," "),
                NameEN = c.CategoryValue.Replace("-", " "),
                CategoryValue = c.CategoryValue,
                ImageUrl = c.Image
            })
            .OrderBy(c => c.NameAR)
            .ToListAsync();
    }

    public async Task<DashboardCategoryDto?> GetCategoryById(int id)
    {
        return await _ctx.CategoryImages
            .Where(c => c.Id == id)
            .Select(c => new DashboardCategoryDto
            {
                Id = c.Id,
                NameAR = c.Category.Replace("-", " "),
                NameEN = c.CategoryValue.Replace("-", " "),
                CategoryValue = c.CategoryValue,
                ImageUrl = c.Image
            })
            .FirstOrDefaultAsync();
    }

  
    public async Task<(bool Success, string? Message, int? CategoryId)> CreateCategory(CreateUpdateCategoryDto dto)
    {
        var CategoryValue = dto.NameEN.Replace(" ", "-").ToLower();
        try
        {
            var exists = await _ctx.CategoryImages
                .AnyAsync(c => c.CategoryValue.ToLower() == CategoryValue);

            if (exists)
                return (false, $"القيمة ({dto.NameAR}, {dto.NameEN}), موجودة مسبقاً", null);

            var category = new CategoryImage
            {
                Category = dto.NameAR.Replace(" ","-"),
                CategoryValue = CategoryValue,
            };

            if (dto.ImageFile != null)
            {
                var imageUrl = await SaveImageFile(dto.ImageFile, "categories");
                category.Image = imageUrl;
            }
            else if (!string.IsNullOrEmpty(dto.ImageUrlLink))
            {
                category.Image = dto.ImageUrlLink;
            }
            else
            {
                return (false, "يجب توفير صورة للقسم", null);
            }

            _ctx.CategoryImages.Add(category);
            await _ctx.SaveChangesAsync();

            return (true, "تم إنشاء القسم بنجاح", category.Id);
        }
        catch (Exception ex)
        {
            return (false, $"خطأ في إنشاء القسم: {ex.Message}", null);
        }
    }

    public async Task<(bool Success, string? Message, int? CategoryId)> UpdateCategory(int id, CreateUpdateCategoryDto dto)
    {
        var CategoryValue=string.Empty;
        if (dto.NameEN is not null)
         CategoryValue = dto.NameEN.Replace(" ", "-").ToLower();
      
        try
        {
            var category = await _ctx.CategoryImages.FindAsync(id);
            if (category == null)
                return (false, "القسم غير موجود",0);

            var exists = await _ctx.CategoryImages
                .AnyAsync(c => c.CategoryValue.ToLower() == CategoryValue && c.Id != id);

            if (exists)
                return (false, $"القيمة ({CategoryValue}) موجودة مسبقاً",0);

            category.Category = dto.NameAR is not null ? dto.NameAR.Replace(" ", "-") : category.Category;
            category.CategoryValue = dto.NameEN is not null ? dto.NameEN.Replace(" ", "-") : category.CategoryValue;

            if (dto.ImageFile != null)
            {
                if (!string.IsNullOrEmpty(category.Image) )//&& !category.Image.StartsWith("http"))
                {
                    DeleteImageFile(category.Image);
                }

                var imageUrl = await SaveImageFile(dto.ImageFile, "categories");
                category.Image = imageUrl;
            }
            else if (!string.IsNullOrEmpty(dto.ImageUrlLink))
            {
                if (!string.IsNullOrEmpty(category.Image))// && !category.Image.StartsWith("http"))
                {
                    DeleteImageFile(category.Image);
                }

                category.Image = dto.ImageUrlLink;
            }

            _ctx.CategoryImages.Update(category);
            await _ctx.SaveChangesAsync();

            return (true, "تم تحديث القسم بنجاح", category.Id);
        }
        catch (Exception ex)
        {
            return (false, $"خطأ في تحديث القسم: {ex.Message}", 0);
        }
    }

    public async Task<(bool Success, string? Message)> DeleteCategory(int id)
    {
        try
        {
            var category = await _ctx.CategoryImages.FindAsync(id);
            if (category == null)
                return (false, "القسم غير موجود");

            var hasProducts = await _ctx.Products.Include(p => p.CategoryImage)
                .AnyAsync(p => p.CategoryImage.CategoryValue.ToLower() == category.CategoryValue.ToLower());

            if (hasProducts)
                return (false, "لا يمكن حذف القسم لأنه يحتوي على منتجات");

            if (!string.IsNullOrEmpty(category.Image))// && !category.Image.StartsWith("http"))
            {
                DeleteImageFile(category.Image);
            }

            _ctx.CategoryImages.Remove(category);
            await _ctx.SaveChangesAsync();

            return (true, "تم حذف القسم بنجاح");
        }
        catch (Exception ex)
        {
            return (false, $"خطأ في حذف القسم: {ex.Message}");
        }
    }

    #region Helper Methods

     
    private async Task<string> SaveImageFile(IFormFile file, string folder)
    {
        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        var uploadPath = Path.Combine(_env.WebRootPath, "images", folder);

        if (!Directory.Exists(uploadPath))
            Directory.CreateDirectory(uploadPath);

        var filePath = Path.Combine(uploadPath, fileName);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var baseUrl = _config["ImageSettings:BaseUrl"];
        return $"{baseUrl}/images/{folder}/{fileName}";
    }

   
    private void DeleteImageFile(string imageUrl)
    {
        try
        {
             var uri = new Uri(imageUrl);
            var relativePath = uri.AbsolutePath.TrimStart('/');
            var imagePath = Path.Combine(_env.WebRootPath, relativePath);

            if (File.Exists(imagePath))
                File.Delete(imagePath);
        }
        catch
        {
         }
    }

    internal async Task<int> GetTotalCategoriesCount()
    {
        return await _ctx.CategoryImages.CountAsync();
    }

    #endregion
}
