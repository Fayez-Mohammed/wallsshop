using Microsoft.EntityFrameworkCore;
using WallsShop.Context;
using WallsShop.DTO;
using WallsShop.Entity;

namespace WallsShop.Repository;

public class OfferRepository(WallShopContext ctx)
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
            EndDate = dto.EndDate
        };

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

    public async Task<List<OfferReturnDto>> GetOffers(string languageCode)
    {
        if (languageCode.ToLower() == "en")
        {
            var offers = await ctx.Offers.Select(a=> new OfferReturnDto{
                Id = a.Id,
                Description = a.Description
                ,Name = a.Name ,CategoryValue = a.CateogryValue}).ToListAsync();
            return offers;
        }
        else
        {
            var offers = await ctx.Offers.
                Select(a=> new OfferReturnDto(){
                    Id = a.Id,
                    Description =a.ArabicDescription,
                    Name = a.ArabicName,CategoryValue = a.CateogryValue}).ToListAsync();
            return offers;
        }
    }
    
    public async Task<bool> DeleteOffer(int offerId)
    {
        var offer = await ctx.Offers.FindAsync(offerId);
        if (offer == null)
            return false;

        try
        {
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