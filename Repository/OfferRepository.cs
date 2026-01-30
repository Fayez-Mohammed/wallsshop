using Microsoft.EntityFrameworkCore;
using WallsShop.Context;
using WallsShop.DTO;
using WallsShop.Entity;
using Microsoft.AspNetCore.Hosting;
namespace WallsShop.Repository;

public class OfferRepository(WallShopContext ctx, IWebHostEnvironment env, IConfiguration config)
{

    public async Task<bool> AddOffer(AddOfferDto dto)
    {
        Offer offer = new Offer
        {
            Name = dto.Name,
            ArabicName = dto.ArabicName,
            ArabicDescription = dto.ArabicDescription,
            Description = dto.Description,
            CateogryValue = dto.CateogryValue,
            StartDate =    dto.StartDate,
            EndDate = dto.EndDate,
            IsActive = true,
        };
        if (dto.ImageFile != null)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.ImageFile.FileName);

            var uploadPath = Path.Combine(env.WebRootPath, "images/offers");

            if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

            var filePath = Path.Combine(uploadPath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.ImageFile.CopyToAsync(stream);
            }
            var baseUrl = config["ImageSettings:BaseUrl"];

            offer.ImageUrl = $"{baseUrl}/images/offers/{fileName}";
          //  offer.ImageUrl = "images/offers/" + fileName;
        }
        else if (!string.IsNullOrEmpty(dto.ImageUrlLink))
        {
            offer.ImageUrl = dto.ImageUrlLink;
        }
        try
        {
            ctx.Offers.Add(offer);
            await ctx.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    //public async Task<List<OfferReturnDto>> GetOffers(string languageCode)
    //{
    //    if (languageCode.ToLower() == "en")
    //    {
    //        var offers = await ctx.Offers.Select(a=> new OfferReturnDto{
    //            Id = a.Id,
    //            Description = a.Description
    //            ,Name = a.Name ,CategoryValue = a.CateogryValue}).ToListAsync();
    //        return offers;
    //    }
    //    else
    //    {
    //        var offers = await ctx.Offers.
    //            Select(a=> new OfferReturnDto(){
    //                Id = a.Id,
    //                Description =a.ArabicDescription,
    //                Name = a.ArabicName,CategoryValue = a.CateogryValue}).ToListAsync();
    //        return offers;
    //    }
    //}
    public async Task<List<OfferReturnDto>> GetOffers(string languageCode)
    {
        // 3. التعديل المهم: بنجيب تاريخ النهاردة
        var today = DateOnly.FromDateTime(DateTime.Now);

        // بنعمل Query أساسي فيه الشرط "السحري" بتاع التاريخ والنشاط
        var query = ctx.Offers
            .Where(o => o.IsActive == true && o.EndDate >= today);

        if (languageCode.ToLower() == "en")
        {
            return await query.Select(a => new OfferReturnDto
            {
                Id = a.Id,
                Description = a.Description,
                Name = a.Name,
                CategoryValue = a.CateogryValue,
                ImageUrl = a.ImageUrl, 
                EndDate = a.EndDate    
            }).ToListAsync();
        }
        else
        {
            return await query.Select(a => new OfferReturnDto
            {
                Id = a.Id,
                Description = a.ArabicDescription,
                Name = a.ArabicName,
                CategoryValue = a.CateogryValue,
                ImageUrl = a.ImageUrl, // الصورة
                EndDate = a.EndDate
            }).ToListAsync();
        }
    }
    public async Task<bool> DeleteOffer(int offerId)
    {
        var offer = await ctx.Offers.FindAsync(offerId);
        if (offer == null)
            return false;

        try
        {

            if (!string.IsNullOrEmpty(offer.ImageUrl) && !offer.ImageUrl.StartsWith("http"))
            {
                var imagePath = Path.Combine(env.WebRootPath, offer.ImageUrl);
                if (File.Exists(imagePath)) File.Delete(imagePath);
            }
            ctx.Offers.Remove(offer);
            await ctx.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}