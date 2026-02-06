using Microsoft.EntityFrameworkCore;
using WallsShop.Context;
using WallsShop.DTO.Dashboard;
using WallsShop.Entity;

namespace WallsShop.Repository.Dashboard;

public class DashboardOfferRepository
{
    private readonly WallShopContext _ctx;
    private readonly IWebHostEnvironment _env;
    private readonly IConfiguration _config;

    public DashboardOfferRepository(
        WallShopContext ctx,
        IWebHostEnvironment env,
        IConfiguration config)
    {
        _ctx = ctx;
        _env = env;
        _config = config;
    }


    public async Task<List<DashboardOfferDto>> GetAllOffers()
    {
        var today = DateOnly.FromDateTime(DateTime.Now);

        return await _ctx.Offers.Include(c=> c.CategoryImage)
            .Select(o => new DashboardOfferDto
            {
                Id = o.Id,
                TitleAR = o.ArabicName,
                TitleEN = o.Name,
                DescriptionAR = o.ArabicDescription,
                DescriptionEN = o.Description,
                StartDate = o.StartDate,
                EndDate = o.EndDate,
                IsActive = o.IsActive,
                IsRunning = o.IsActive && o.EndDate >= today,
                CategoryAR = o.CategoryImage.Category.Replace("-", " "),
                CategoryEN = o.CategoryImage.CategoryValue.Replace("-", " "),
                CategoryValue = o.CategoryImage.CategoryValue,
                ImageUrl = o.ImageUrl
            })
            .OrderByDescending(o => o.Id)
            .ToListAsync();
    }

  
    public async Task<DashboardOfferDto?> GetOfferById(int id)
    {
        var today = DateOnly.FromDateTime(DateTime.Now);

        return await _ctx.Offers.Include(c => c.CategoryImage)
            .Where(o => o.Id == id)
            .Select(o => new DashboardOfferDto
            {
                Id = o.Id,
                TitleAR = o.ArabicName,
                TitleEN = o.Name,
                DescriptionAR = o.ArabicDescription,
                DescriptionEN = o.Description,
                StartDate = o.StartDate,
                EndDate = o.EndDate,
                IsActive = o.IsActive,
                IsRunning = o.IsActive && o.EndDate >= today,
                CategoryAR = o.CategoryImage.Category.Replace("-", " "),
                CategoryEN = o.CategoryImage.CategoryValue.Replace("-", " "),
                CategoryValue = o.CategoryImage.CategoryValue,
                ImageUrl = o.ImageUrl
            })
            .FirstOrDefaultAsync();
    }


    public async Task<(bool Success, string? Message, int? OfferId)> CreateOffer(CreateUpdateOfferDto dto)
    {
        try
        {
            var Category = await _ctx.CategoryImages
                .FirstOrDefaultAsync(c => c.CategoryValue == dto.CategoryValue.Replace(" ","-"));
            var offer = new Offer
            {
                ArabicName = dto.TitleAR,
                Name = dto.TitleEN,
                ArabicDescription = dto.DescriptionAR,
                Description = dto.DescriptionEN,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                IsActive = dto.IsActive,
                CategoryFK= Category != null ? Category.Id : null,
                //CateogryValue = dto.CategoryValue
            };

            if (dto.ImageFile != null)
            {
                var imageUrl = await SaveImageFile(dto.ImageFile, "offers");
                offer.ImageUrl = imageUrl;
            }
            else if (!string.IsNullOrEmpty(dto.ImageUrlLink))
            {
                offer.ImageUrl = dto.ImageUrlLink;
            }

            _ctx.Offers.Add(offer);
            await _ctx.SaveChangesAsync();

            return (true, "تم إنشاء العرض بنجاح", offer.Id);
        }
        catch (Exception ex)
        {
            return (false, $"خطأ في إنشاء العرض: {ex.Message}", null);
        }
    }


    public async Task<(bool Success, string? Message,int? OfferId)> UpdateOffer(int id, CreateUpdateOfferDto dto)
    {
        try
        {
            var Category = await _ctx.CategoryImages
                .FirstOrDefaultAsync(c => c.CategoryValue == dto.CategoryValue.Replace(" ", "-"));
            var offer = await _ctx.Offers.FindAsync(id);
            if (offer == null)
                return (false, "العرض غير موجود",0);

            offer.ArabicName = dto.TitleAR;
            offer.Name = dto.TitleEN;
            offer.ArabicDescription = dto.DescriptionAR;
            offer.Description = dto.DescriptionEN;
            offer.StartDate = dto.StartDate;
            offer.EndDate = dto.EndDate;
            offer.IsActive = dto.IsActive;
            offer.CategoryFK = Category != null ? Category.Id : null;

            if (dto.ImageFile != null)
            {
                if (!string.IsNullOrEmpty(offer.ImageUrl) && !offer.ImageUrl.StartsWith("http"))
                {
                    DeleteImageFile(offer.ImageUrl);
                }

                var imageUrl = await SaveImageFile(dto.ImageFile, "offers");
                offer.ImageUrl = imageUrl;
            }
            else if (!string.IsNullOrEmpty(dto.ImageUrlLink))
            {
                if (!string.IsNullOrEmpty(offer.ImageUrl) && !offer.ImageUrl.StartsWith("http"))
                {
                    DeleteImageFile(offer.ImageUrl);
                }

                offer.ImageUrl = dto.ImageUrlLink;
            }

            _ctx.Offers.Update(offer);
            await _ctx.SaveChangesAsync();

            return (true, "تم تحديث العرض بنجاح", offer.Id);
        }
        catch (Exception ex)
        {
            return (false, $"خطأ في تحديث العرض: {ex.Message}", 0);
        }
    }

 
    public async Task<(bool Success, string? Message)> DeleteOffer(int id)
    {
        try
        {
            var offer = await _ctx.Offers.FindAsync(id);
            if (offer == null)
                return (false, "العرض غير موجود");

            if (!string.IsNullOrEmpty(offer.ImageUrl) && !offer.ImageUrl.StartsWith("http"))
            {
                DeleteImageFile(offer.ImageUrl);
            }

            _ctx.Offers.Remove(offer);
            await _ctx.SaveChangesAsync();

            return (true, "تم حذف العرض بنجاح");
        }
        catch (Exception ex)
        {
            return (false, $"خطأ في حذف العرض: {ex.Message}");
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

    internal async Task<int> GetTotalOffersCount()
    {
        
        return await _ctx.Offers.CountAsync();
    }

    public async Task<List<Offer>> GetAllOffersAsync()
    {
      return  await _ctx.Offers.ToListAsync();
    }

    #endregion
}
