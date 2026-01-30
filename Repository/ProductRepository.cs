using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WallsShop.Context;
using WallsShop.DTO;
using WallsShop.Entity;
using WallsShop.Properties.Entity;
 

namespace WallsShop.Repository;

public class ProductRepository(WallShopContext ctx)
{
    public async Task<List<ProductResponseDto>> GetArabicProductById(QueryParameters queryParameters, string? userId = null)
    {
        var query = ctx.Products.AsQueryable();
        //////////////
        var wishlistIds = await GetWishlistIdsAsync(userId, queryParameters.Ids);
        /////////////////////
        if (!string.IsNullOrEmpty(queryParameters.category))
        {
            var cat = queryParameters.category.ToLower();
            query = query.Where(a => a.Category.ToLower() == cat);
        }
        
        if (queryParameters.id > 0)
        {
            query = query.Where(a => a.Id == queryParameters.id);
        }


        query = (queryParameters.order ?? "").ToLower() switch
        {
            "price_asc" => query.OrderBy(a => a.Price),
            "price_desc" => query.OrderByDescending(a => a.Price),
            "rating_desc" => query.OrderByDescending(a => a.AverageRate),
            "rating_asc" => query.OrderBy(a => a.AverageRate),
            "latest_asc" => query.OrderBy(a=>a.Created),
            "latest_desc" => query.OrderByDescending(a => a.Created),
            _ => query.OrderBy(a => a.Id) // Default ordering is good practice for pagination
        };

       
        if (queryParameters.page > 0 && queryParameters.pageSize > 0)
        {
            var skip = (queryParameters.page - 1) * queryParameters.pageSize;
            query = query.Skip(skip).Take(queryParameters.pageSize);
        }

     
        return await query.Select(p => new ProductResponseDto
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            PriceAfterDiscount = p.PriceAfterDiscount,
            Description = p.Descriptions,
            ShortDescription = p.FullDescription,
            Category = p.Category.Replace("-"," "),
            CateogryValue = p.CategoryValue,
            SKU = p.SKU,
            IsInWishList = wishlistIds.Contains(p.Id),
            Images = p.Images.Select(img => new ProductImageDto
            {
                Path = img.RelativePath
            }).ToList(),
            Colors = p.Colors.Where(a=>a.LanguageCode == "ar").Select(c => c.Color).ToList(),
            Variants = p.Variants
                .Where(a=>a.LanguageCode == "ar")
                .Select(v => new ProductVariantDto
            {
                Id = v.Id,
                Type = v.Type,
                Size = v.Size,
                Price = v.Price,
                PriceBeforeDiscount = v.PriceBeforeDiscount ?? 0
            }).ToList()
        }).ToListAsync();
    }

    public async Task<List<ProductTranslationDto>> GetEnglishProductById(QueryParameters queryParameters, string? userId = null)
    {
 
        var query = ctx
            .ProductTranslations
            .Include(t => t.Product)            
            .ThenInclude(p => p.Variants)   
            .Include(t => t.Product)
            .ThenInclude(t=>t.Images)
            .AsQueryable();
        var wishlistIds = await GetWishlistIdsAsync(userId, queryParameters.Ids);

        if (queryParameters.id > 0)
        {
            query = query.Where(a => a.Id ==queryParameters.id);
        }

        if (!string.IsNullOrEmpty(queryParameters.category))
        {
            query = query.Where(a => a.Category.ToLower() == queryParameters.category.ToLower());
        }
        
        if (queryParameters.page > 0 && queryParameters.pageSize > 0)
        {
            var skip = (queryParameters.page - 1) * queryParameters.pageSize;
            query = query.Skip(skip).Take(queryParameters.pageSize);
        }
         
        
        query = queryParameters.order.ToLower() switch
        {
            "price_asc" => query.OrderBy(a => a.Price),
            "price_desc" => query.OrderByDescending(a => a.Price),
            "rating_desc" => query.OrderByDescending(a => a.AverageRate),
            "rating_asc" => query.OrderBy(a => a.AverageRate),
			"latest_asc" => query.OrderBy(a => a.Product.Created),
			"latest_desc" => query.OrderByDescending(a => a.Product.Created),
			_ => query
        };

        return  await query.Select(pt => new ProductTranslationDto{
                Id = pt.Id,
                ProductId = pt.ProductId,
                Name = pt.Name,
                ShortDescription = pt.Description.Split(new string []
                {
                     "\n"
                },StringSplitOptions.RemoveEmptyEntries)[0],
                Descriptions = pt.Description.Split(new[] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries).ToList() ,
                Category = pt.Category.Replace("-"," "),
                AverageRate = pt.Product.AverageRate,
                Variants = pt.Product.Variants.Where(a=>a.LanguageCode == "en").ToList(),
                Price = pt.Product.Price,
                PriceAfterDiscount = pt.Product.PriceAfterDiscount,
                SKU = pt.Product.SKU,
            IsInWishList = wishlistIds.Contains(pt.Id),
            Images = pt.Product.Images.Select(img => new ProductImageDto()
                {
                    Path = img.RelativePath
                }).ToList(),
                Colors = pt.Product.Colors.Where(a=>a.LanguageCode == "en").Select(c => c.Color).ToList()
            })
            .ToListAsync<ProductTranslationDto>();;
    }

    public async Task<List<ProductOverviewDto>> GetProductsOverview(QueryParameters queryParameters, string? userId = null)
    {
        queryParameters.page = queryParameters.page > 0 ? queryParameters.page   :  1;
        queryParameters.pageSize = queryParameters.pageSize > 0 ? queryParameters.pageSize : 12;
        
        var query = ctx
            .Products
            .Include(p => p.Variants)   
            .Include(t=>t.Images)
            .AsQueryable();


        var wishlistIds = await GetWishlistIdsAsync(userId, queryParameters.Ids);

        if (!string.IsNullOrEmpty(queryParameters.category))
        {
            query = query.Where(a => a.Category.ToLower() == queryParameters.category.ToLower());
        }
        
        if (queryParameters.page > 0 && queryParameters.pageSize > 0)
        {
            var skip = (queryParameters.page - 1) * queryParameters.pageSize;
            query = query.Skip(skip).Take(queryParameters.pageSize);
        }
         
        
        query = queryParameters.order.ToLower() switch
        {
            "price_asc" => query.OrderBy(a => a.Price),
            "price_desc" => query.OrderByDescending(a => a.Price),
            "rating_desc" => query.OrderByDescending(a => a.AverageRate),
            "rating_asc" => query.OrderBy(a => a.AverageRate),
            "latest_asc" => query.OrderBy(a=>a.Created),
            "latest_desc" => query.OrderByDescending(a => a.Created),
            _ => query
        };
         
        var productOverviews =  query.Select(p => new ProductOverviewDto
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            PriceAfterDiscount = p.PriceAfterDiscount,
            ImageUrl = p.Images.FirstOrDefault().RelativePath,
            TotalRatingPeople = p.TotalRatePeople,
            AverageRatingPeople = p.AverageRate,
            IsInWishList = wishlistIds.Contains(p.Id),
        }).ToList();

        return productOverviews;
    }
    
    
    public async Task<List<ProductTranslationOverviewDto>> GetProductsTranslationOverview(QueryParameters queryParameters, string? userId = null)
    {
        queryParameters.page = queryParameters.page > 0 ? queryParameters.page   :  1;
        queryParameters.pageSize = queryParameters.pageSize > 0 ? queryParameters.pageSize : 12;
         
         
        var query = ctx
            .ProductTranslations
            .Include(t => t.Product)            
            .ThenInclude(p => p.Variants)   
            .Include(p=>p.Product)
            .ThenInclude(t=>t.Images)
            .AsQueryable();

        var wishlistIds = await GetWishlistIdsAsync(userId, queryParameters.Ids);

        if (!string.IsNullOrEmpty(queryParameters.category))
        {
            query = query.Where(a => a.Category.ToLower() == queryParameters.category.ToLower());
        }
        
        if (queryParameters.page > 0 && queryParameters.pageSize > 0)
        {
            var skip = (queryParameters.page - 1) * queryParameters.pageSize;
            query = query.Skip(skip).Take(queryParameters.pageSize);
        }
         
        
        query = queryParameters.order.ToLower() switch
        {
            "price_asc" => query.OrderBy(a => a.Price),
            "price_desc" => query.OrderByDescending(a => a.Price),
            "rating_desc" => query.OrderByDescending(a => a.AverageRate),
            "rating_asc" => query.OrderBy(a => a.AverageRate),
            _ => query
        };

        
        var productOverviews =  query.Select(p => 
            new ProductTranslationOverviewDto
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            PriceAfterDiscount = p.PriceAfterDiscount,
            ImageUrl = p.Product.Images.FirstOrDefault().RelativePath,
            TotalPeopleRating = p.Product.TotalRatePeople,
            AverageRatingPeople = p.Product.AverageRate,
             IsInWishList = wishlistIds.Contains(p.Id)
            }).ToList();

        return productOverviews;
    }

    public async Task<List<ProductOverviewDto>> GetTop4RatedProducts(QueryParameters queryParameters, string? userId = null)
    {
        var wishlistIds = await GetWishlistIdsAsync(userId, queryParameters.Ids);

        return await ctx?.Products
            .OrderByDescending(a => a.AverageRate)
            .Include(a=>a.Variants)
            .Include(a => a.Images)
            .Take(4)
            .Select(p => new ProductOverviewDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                PriceAfterDiscount = p.PriceAfterDiscount,
                ImageUrl = p.Images.FirstOrDefault().RelativePath,
                TotalRatingPeople = p.TotalRatePeople,
                AverageRatingPeople = p.AverageRate,
                IsInWishList = wishlistIds.Contains(p.Id)
            }).ToListAsync();
             
    }

    public async Task<List<ProductOverviewDto>> GetTop4RecentProducts(QueryParameters query, string? userId = null)
    {
        var wishlistIds = await GetWishlistIdsAsync(userId, query.Ids);

        return await ctx?.Products
            .OrderByDescending(a => a.Created)
            .Include(a => a.Variants)
            .Include(a => a.Images)
            .OrderByDescending(a=>a.Created)
            .Take(4)
            .Select(p => new ProductOverviewDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                PriceAfterDiscount = p.PriceAfterDiscount,
                ImageUrl = p.Images.FirstOrDefault().RelativePath,
                TotalRatingPeople = p.TotalRatePeople,
                AverageRatingPeople = p.AverageRate,
                  IsInWishList = wishlistIds.Contains(p.Id)
            }).ToListAsync();
    }

    public async Task<List<ProductTranslationOverviewDto>> GetTop4RecentProductTranslations(QueryParameters queryParameters, string? userId = null)
    {
        var wishlistIds = await GetWishlistIdsAsync(userId, queryParameters.Ids);
                    
        return await ctx?
            .ProductTranslations
            .Include(t => t.Product)
            .ThenInclude(p => p.Variants)  
            .Include(t => t.Product)
            .ThenInclude(p => p.Images)    
            .OrderByDescending(t => t.Product.Created)
            .Take(4)
            .Select(pt => new ProductTranslationOverviewDto{
                Id = pt.Id,
                Name = pt.Name,
                Price = pt.Price,
                PriceAfterDiscount = pt.PriceAfterDiscount,
                ImageUrl = pt.Product.Images.FirstOrDefault().RelativePath,
                TotalPeopleRating = pt.Product.TotalRatePeople,
                AverageRatingPeople = pt.Product.AverageRate,
                 IsInWishList = wishlistIds.Contains(pt.Id)
            })
            .ToListAsync<ProductTranslationOverviewDto>();
    }

    public async Task<List<ProductTranslationOverviewDto>> GetTop4RatedProductTranslations(QueryParameters queryParameters, string? userId = null)
    {
        var wishlistIds = await GetWishlistIdsAsync(userId, queryParameters.Ids);

        return await ctx?.ProductTranslations
            .Include(t => t.Product).ThenInclude(p => p.Variants)
            .Include(t => t.Product).ThenInclude(p => p.Images)
            .OrderByDescending(t => t.Product.AverageRate)
            .Take(4)
            .Select(pt => new ProductTranslationOverviewDto(){
                Id = pt.Id,
                Name = pt.Name,
                Price = pt.Price,
                PriceAfterDiscount = pt.PriceAfterDiscount,
                ImageUrl = pt.Product.Images.FirstOrDefault().RelativePath,
                TotalPeopleRating = pt.Product.TotalRatePeople,
                AverageRatingPeople = pt.Product.AverageRate,
                 IsInWishList = wishlistIds.Contains(pt.Id)
            })
            .ToListAsync<ProductTranslationOverviewDto>();
    }
    
    public List<string> GetProductTranslationCategories()
    {
        return ctx.ProductTranslations
            .Select(t => t.Product.Category)
            .Distinct()
            .ToList();
    }

    public async Task<List<ProductOverviewDto>> GetProductsByCategory(QueryParameters queryParameters , int Id, string? userId = null)
    {
        var wishlistIds = await GetWishlistIdsAsync(userId, queryParameters.Ids);

        return await ctx.Products
            .Where(a => a.Category.ToLower() == queryParameters.category.ToLower() && a.Id != Id)
            .Include(a => a.Images)
            .Take(4)
            .Select(p => new ProductOverviewDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                PriceAfterDiscount = p.PriceAfterDiscount,
                ImageUrl = p.Images.FirstOrDefault().RelativePath,
                TotalRatingPeople = p.TotalRatePeople,
                AverageRatingPeople = p.AverageRate,
                IsInWishList = wishlistIds.Contains(p.Id)
            }).ToListAsync();
    }


    public async Task<List<ProductTranslationOverviewDto>> GetProductTranslationsByCategory(QueryParameters queryParameters, int Id, string? userId = null)
    {
        var wishlistIds = await GetWishlistIdsAsync(userId, queryParameters.Ids);

        return await ctx.ProductTranslations
            .Where(a => a.Category.ToLower() == queryParameters.category.ToLower() && a.Id != Id)
            .Include(a=>a.Product)
            .ThenInclude(a => a.Images)
            .Take(4)
            .Select(pt => new ProductTranslationOverviewDto(){
                Id = pt.Id,
                Name = pt.Name,
                Price = pt.Product.Price,
                PriceAfterDiscount = pt.Product.PriceAfterDiscount,
                TotalPeopleRating = pt.Product.TotalRatePeople,
                AverageRatingPeople = pt.Product.AverageRate,
                IsInWishList = wishlistIds.Contains(pt.Id)
            })
            .ToListAsync<ProductTranslationOverviewDto>();
    }

    public void SaveChanges()
    {
        ctx.SaveChanges();
    }
    // ends here 
    public bool UpdateProduct(Product product)
    {
        try
        {
            ctx.Products.Update(product);
            return ctx.SaveChanges() > 0;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

     

    public bool AddProduct(ProductAddDto productDto)
    {
        try
        {
            var product = new Product
            {
                Name = productDto.Name,
                FullDescription = productDto.Description,
                PriceAfterDiscount = productDto.PriceBeforeDiscount != null
                    ? productDto.PriceBeforeDiscount - productDto.Price
                    : 0,
                Price = productDto.Price,
                Category = productDto.Category,
                SKU = productDto.SKU,
                Created = DateTime.UtcNow,
                LanguageCode = productDto.LanguageCode,


                Variants = new List<Variant>(),
                Colors = new List<ProductColor>()
            };


            foreach (var se in productDto.Size)
            {
                var types = productDto.Type.Any() ? productDto.Type : new List<string> { "" };

                foreach (var type in types)
                {
                    product.Variants.Add(new Variant
                    {
                        SKU = productDto.SKU,
                        Size = se,
                        Type = type,
                        Price = productDto.Price,
                        LanguageCode = productDto.LanguageCode,
                        PriceBeforeDiscount = productDto.PriceBeforeDiscount,
                        DiscountRate = productDto.PriceBeforeDiscount > 0
                            ? Math.Round(((productDto.PriceBeforeDiscount - productDto.Price)
                                          / productDto.PriceBeforeDiscount) * 100, 2)
                            : 0
                    });
                }
            }

            productDto.Colors.ForEach(color =>
            {
                product.Colors.Add(new ProductColor
                {
                    Color = color
                });
            });


            ctx.Products.Add(product);


            return ctx.SaveChanges() > 0;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public bool DeleteProduct(int id)
    {
        try
        {
            var product = ctx.Products.Find(id);

            if (product == null)
                return false;

            ctx.Products.Remove(product);

            return ctx.SaveChanges() > 0;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public async Task<List<ImageFileResult>> GetImagesByProductIdAsync(int productId, IWebHostEnvironment _env)
    {
       
        var imageRecords = await ctx.Images
            .Where(a => a.ProductId == productId)
            .ToListAsync();

        var results = new List<ImageFileResult>();

        
        foreach (var image in imageRecords)
        {
            
            results.Add(new ImageFileResult
            {
                FullPath = image.RelativePath,
                ContentType = "image/" + Path.GetExtension(image.RelativePath).TrimStart('.'),
                FileName = Path.GetFileName(image.RelativePath)
            });
        }

        return results; 
    }



    //////////////////////////
    ///
    // دالة مساعدة (Helper Method)
    private async Task<List<int>> GetWishlistIdsAsync(string? userId, List<int>? frontendIds)
    {
        if (!string.IsNullOrEmpty(userId))
        {
            return await ctx.Wishlists
                .Where(w => w.UserId == userId)
                .Select(w => w.ProductId)
                .ToListAsync();
        }

        if (frontendIds != null && frontendIds.Any())
        {
            return frontendIds;
        }

        return new List<int>();
    }
}

