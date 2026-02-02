using Microsoft.EntityFrameworkCore;
using WallsShop.Context;
using WallsShop.DTO;
using WallsShop.Entity;

namespace WallsShop.Repository;

public class WishlistRepository(WallShopContext ctx )
{
    public async Task<List<ProductOverviewDto>> GetWishlist(string userId, string LanguageCode)
    {
        var ids = await ctx
            .Wishlists
            .Where(w => w.UserId == userId)
            .Select(a => a.ProductId)
            .ToListAsync();
        if (LanguageCode == "ar") { 
            var products = await ctx
                .Products
                .Where(p => ids.Contains(p.Id))
                .Select(p => new ProductOverviewDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    PriceAfterDiscount = p.PriceAfterDiscount,
                    ImageUrl = p.Images.Select(i => i.RelativePath).FirstOrDefault() ?? "No image",
                    IsInWishList = true
                })
                .ToListAsync();

        return products; }
        else
        {
            var products = await ctx.ProductTranslations
                .Where(pt => ids.Contains(pt.ProductId))
                .Select(pt => new ProductOverviewDto
                {
                    Id = pt.ProductId,
                    Name = pt.Name,
                    Price = pt.Product.Price,
                    PriceAfterDiscount = pt.Product.PriceAfterDiscount,
                    ImageUrl = pt.Product.Images.Select(i => i.RelativePath).FirstOrDefault() ?? "No image",
                    IsInWishList = true
                })
                .ToListAsync();
            return products;
        }
    }
    public async Task AddToWishlist(WhishlistDto wishlist, string userId)
    {
        if (wishlist?.ProductIds == null || !wishlist.ProductIds.Any())
            return;

        // Fetch all existing ProductIds for this user in one query
        var existingProductIds = await ctx.Wishlists
            .Where(w => w.UserId == userId && wishlist.ProductIds.Contains(w.ProductId))
            .Select(w => w.ProductId)
            .ToListAsync();

        // Only add new ProductIds
        var newItems = wishlist.ProductIds
            .Except(existingProductIds)
            .Select(pid => new Wishlist
            {
                UserId = userId,
                ProductId = pid
            });
        ctx.ChangeTracker.Clear();
        // Add all new items at once
        await ctx.Wishlists.AddRangeAsync(newItems);

        await ctx.SaveChangesAsync();
    }


    
    public async Task<List<ProductOverviewDto>> GetProducts(WhishlistDto wishlistDto)
    {
        // product id 
        var products = await ctx
            .Products
            .Where(p => wishlistDto.ProductIds.Contains(p.Id))
            .Select(p => new ProductOverviewDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.PriceAfterDiscount,
                PriceAfterDiscount = p.PriceAfterDiscount,
                ImageUrl = p.Images.Select(i => i.RelativePath).FirstOrDefault() ?? "No image",
                IsInWishList = true
            })
            .ToListAsync();
        return products;
    }

    public async Task RemoveFromWishlist(string userId, int productId)
    {
        var wishlistItem = await ctx.Wishlists
            .FirstOrDefaultAsync(wl => wl.UserId == userId && wl.ProductId == productId);
        if (wishlistItem != null)
        {
            ctx.Wishlists.Remove(wishlistItem);
            await ctx.SaveChangesAsync();
        }
    }

    public int CountOFWishlistForSpecificUser(string userId)
    {
        var wishlistItemsCount= ctx.Wishlists.Where(w=>w.UserId==userId).Count();

        return wishlistItemsCount;
    }
}