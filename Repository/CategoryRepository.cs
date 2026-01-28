using Microsoft.EntityFrameworkCore;
using WallsShop.Context;
using WallsShop.DTO;
using WallsShop.Entity;

namespace WallsShop.Repository;

public class CategoryRepository(WallShopContext ctx)
{
    public async Task<List<CategoryResponseDto>> GetCategories (string languageCode)
    {
        return await ctx.CategoryImages.Select(a=> new CategoryResponseDto()
            {
                Category = languageCode == "en" ? a.CategoryValue.Replace("-"," ") : a.Category.Replace("-"," "),
                CategoryValue = a.CategoryValue,
                CategoryImage = a.Image
            }
        ).ToListAsync();
    }
   
}