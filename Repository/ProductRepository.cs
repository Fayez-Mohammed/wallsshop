using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WallsShop.Context;
using WallsShop.DTO;
using WallsShop.Entity;
using WallsShop.Extensions;
using WallsShop.Properties.Entity;
 

namespace WallsShop.Repository;

public class ProductRepository(WallShopContext ctx)
{
    public async Task<PagedResult<ProductResponseDto>> GetArabicProductById(QueryParameters queryParameters, string? userId = null)
    {
        var query = ctx.Products.AsQueryable();
        query.Include(c => c.CategoryImage);
        if (queryParameters.search != null)
        {
           
                query = query.Where(a => a.Name.Contains(queryParameters.search) || a.Descriptions.Contains(queryParameters.search));
            
           

        }
        string categoryNameDisplay = "All Categories";
        //if (!string.IsNullOrEmpty(queryParameters.category))
        //{
        //    var cat = queryParameters.category.ToLower();
        //    query = query.Where(a => a.Category.ToLower() == cat);
        //    categoryNameDisplay = queryParameters.category;
        //}
        if (!string.IsNullOrEmpty(queryParameters.category))
        {
            var cat = queryParameters.category.ToLower();
            query = query.Where(a => a.CategoryImage.CategoryValue.ToLower() == cat);
            var category = await ctx.CategoryImages.FirstOrDefaultAsync(c => c.CategoryValue.ToLower() == cat);
            if (category != null)
            {
                categoryNameDisplay = queryParameters.LanguageCode == "en" ? category.CategoryValue.Replace("-", " ") : category.Category.Replace("-", " ");

            }
            else
            {
                categoryNameDisplay = queryParameters.category;
            }
        }
        int currentPage = queryParameters.page > 0 ? queryParameters.page : 1;

        //////////////
        var wishlistIds = await GetWishlistIdsAsync(userId, queryParameters.Ids);
        /////////////////////
      
        
        if (queryParameters.id > 0)
        {
            query = query.Where(a => a.Id == queryParameters.id);
        }
        //var totalCount = await query.CountAsync();

        //var pageSize = queryParameters.pageSize > 0 ? queryParameters.pageSize : 10;
        //var currentPage = queryParameters.page > 0 ? queryParameters.page : 1;

        //var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

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


        //if (queryParameters.page > 0 && queryParameters.pageSize > 0)
        //{
        //    var skip = (queryParameters.page - 1) * queryParameters.pageSize;
        //    query = query.Skip(skip).Take(queryParameters.pageSize);
        //}


        //return await query.Select(p => new ProductResponseDto
        //{
        //    Id = p.Id,
        //    Name = p.Name,
        //    Price = p.Price,
        //    PriceAfterDiscount = p.PriceAfterDiscount,
        //    Description = p.Descriptions,
        //    ShortDescription = p.FullDescription,
        //    Category = p.Category.Replace("-"," "),
        //    CateogryValue = p.CategoryValue,
        //    SKU = p.SKU,
        //    IsInWishList = wishlistIds.Contains(p.Id),
        //    Images = p.Images.Select(img => new ProductImageDto
        //    {
        //        Path = img.RelativePath
        //    }).ToList(),
        //    Colors = p.Colors.Where(a=>a.LanguageCode == "ar").Select(c => c.Color).ToList(),
        //    Variants = p.Variants
        //        .Where(a=>a.LanguageCode == "ar")
        //        .Select(v => new ProductVariantDto
        //    {
        //        Id = v.Id,
        //        Type = v.Type,
        //        Size = v.Size,
        //        Price = v.Price,
        //        PriceBeforeDiscount = v.PriceBeforeDiscount ?? 0
        //    }).ToList()
        //}).ToListAsync();



        var dtoQuery = query.Select(p => new ProductResponseDto
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            PriceAfterDiscount = p.PriceAfterDiscount,
            //   Description = p.Descriptions.Replace("\r\n", "\n"),
            Descriptions = p.Descriptions
    .Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries)
    .ToList(),

            AverageRate= p.AverageRate,


            ShortDescription = p.FullDescription,
            Category = p.CategoryImage.Category.Replace("-", " "),
            CateogryValue = p.CategoryImage.CategoryValue,
            SKU = p.SKU,

            PageNumber = currentPage, 

            IsInWishList = wishlistIds.Contains(p.Id),
            Images = p.Images.Select(img => new ProductImageDto { Path = img.RelativePath }).ToList(),
            Colors = p.Colors.Where(a => a.LanguageCode == "ar").Select(c => c.Color).ToList(),
            Variants = p.Variants.Where(a => a.LanguageCode == "ar")
                .Select(v => new ProductVariantDto
                {
                    Id = v.Id,
                    Type = v.Type,
                    Size = v.Size,
                    Price = v.Price,
                    PriceBeforeDiscount = v.PriceBeforeDiscount ?? 0
                }).ToList()
        });

        return await dtoQuery.ToPagedListAsync(
            queryParameters.page,
            queryParameters.pageSize,
            categoryNameDisplay
        );
    }

    public async Task<PagedResult<ProductTranslationDto>> GetEnglishProductById(QueryParameters queryParameters, string? userId = null)
    {

        var query = ctx
            .ProductTranslations
            .Include(t => t.Product)
            .ThenInclude(p => p.Variants)
            .Include(t => t.Product)
            .ThenInclude(t => t.Images)
            .Include(t => t.Product).ThenInclude(c => c.CategoryImage)
            .AsQueryable();
        if (queryParameters.search != null)
        {
            
                var translatedProductIds = await ctx.ProductTranslations
                    .Where(t => t.Name.Contains(queryParameters.search) || t.Description.Contains(queryParameters.search))
                    .Select(t => t.ProductId)
                    .ToListAsync();
                query = query.Where(a => translatedProductIds.Contains(a.Id));
            

        }
        string categoryNameDisplay = "All Categories";
        //if (!string.IsNullOrEmpty(queryParameters.category))
        //{
        //    var cat = queryParameters.category.ToLower();
        //    query = query.Where(a => a.Category.ToLower() == cat);
        //    categoryNameDisplay = queryParameters.category;
        //}
        if (!string.IsNullOrEmpty(queryParameters.category))
        {
            var cat = queryParameters.category.ToLower();
            query = query.Where(a => a.Product.CategoryImage.Category.ToLower() == cat);
            var category = await ctx.CategoryImages.FirstOrDefaultAsync(c => c.CategoryValue.ToLower() == cat);
            if (category != null)
            {
                categoryNameDisplay = queryParameters.LanguageCode == "en" ? category.CategoryValue.Replace("-", " ") : category.Category.Replace("-", " ");

            }
            else
            {
                categoryNameDisplay = queryParameters.category;
            }
        }
        int currentPage = queryParameters.page > 0 ? queryParameters.page : 1;
        var wishlistIds = await GetWishlistIdsAsync(userId, queryParameters.Ids);

        if (queryParameters.id > 0)
        {
            query = query.Where(a => a.Id ==queryParameters.id);
        }

        //if (!string.IsNullOrEmpty(queryParameters.category))
        //{
        //    query = query.Where(a => a.Category.ToLower() == queryParameters.category.ToLower());
        //}
        
        //if (queryParameters.page > 0 && queryParameters.pageSize > 0)
        //{
        //    var skip = (queryParameters.page - 1) * queryParameters.pageSize;
        //    query = query.Skip(skip).Take(queryParameters.pageSize);
        //}
         
        
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
        var dtoQuery = query.Select(pt => new ProductTranslationDto
        {
            Id = pt.Id,
           // ProductId = pt.ProductId,
            Name = pt.Name,
            ShortDescription = pt.Description.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries)[0],
            Descriptions = pt.Description.Split(new[] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries).ToList(),
            Category = pt.Product.CategoryImage.CategoryValue.Replace("-", " "),
            CateogryValue = pt.Product.CategoryImage.CategoryValue,
            AverageRate = pt.Product.AverageRate,

            PageNumber = currentPage,

            //Variants = pt.Product.Variants.Where(a => a.LanguageCode == "ar").ToList(),
            Variants = pt.Product.Variants.Where(a => a.LanguageCode == "ar")
                .Select(v => new ProductVariantDto
                {
                    Id = v.Id,
                    Type = v.EnglishType,
                    Size = v.EnglishSize,
                    Price = v.Price,
                    PriceBeforeDiscount = v.PriceBeforeDiscount ?? 0
                }).ToList(),
       
        Price = pt.Product.Price,
            PriceAfterDiscount = pt.Product.PriceAfterDiscount,
            SKU = pt.Product.SKU,
            IsInWishList = wishlistIds.Contains(pt.Id), 
            Images = pt.Product.Images.Select(img => new ProductImageDto()
            {
                Path = img.RelativePath
            }).ToList(),
            Colors = pt.Product.Colors.Where(a => a.LanguageCode == "en").Select(c => c.Color).ToList()
        });

        return await dtoQuery.ToPagedListAsync(queryParameters.page, queryParameters.pageSize, categoryNameDisplay);
        //return  await query.Select(pt => new ProductTranslationDto{
        //        Id = pt.Id,
        //        ProductId = pt.ProductId,
        //        Name = pt.Name,
        //        ShortDescription = pt.Description.Split(new string []
        //        {
        //             "\n"
        //        },StringSplitOptions.RemoveEmptyEntries)[0],
        //        Descriptions = pt.Description.Split(new[] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries).ToList() ,
        //        Category = pt.Category.Replace("-"," "),
        //        AverageRate = pt.Product.AverageRate,
        //        Variants = pt.Product.Variants.Where(a=>a.LanguageCode == "en").ToList(),
        //        Price = pt.Product.Price,
        //        PriceAfterDiscount = pt.Product.PriceAfterDiscount,
        //        SKU = pt.Product.SKU,
        //    IsInWishList = wishlistIds.Contains(pt.Id),
        //    Images = pt.Product.Images.Select(img => new ProductImageDto()
        //        {
        //            Path = img.RelativePath
        //        }).ToList(),
        //        Colors = pt.Product.Colors.Where(a=>a.LanguageCode == "en").Select(c => c.Color).ToList()
        //    })
        //    .ToListAsync<ProductTranslationDto>();;
    }

    //public async Task<PagedResult<ProductOverviewDto>> GetProductsOverview(QueryParameters queryParameters, string? userId = null)
    //{
    //  //  queryParameters.page = queryParameters.page > 0 ? queryParameters.page   :  1;
    //    queryParameters.pageSize = queryParameters.pageSize > 0 ? queryParameters.pageSize : 12;

    //    var query = ctx
    //        .Products
    //        .Include(p => p.Variants)   
    //        .Include(t=>t.Images)
    //        .AsQueryable();

    //    string categoryNameDisplay = "All Categories";
    //    if (!string.IsNullOrEmpty(queryParameters.category))
    //    {
    //        var cat = queryParameters.category.ToLower();
    //        query = query.Where(a => a.Category.ToLower() == cat);
    //        categoryNameDisplay = queryParameters.category;
    //    }
    //    int currentPage = queryParameters.page > 0 ? queryParameters.page : 1;
    //    var wishlistIds = await GetWishlistIdsAsync(userId, queryParameters.Ids);

    //    if (!string.IsNullOrEmpty(queryParameters.category))
    //    {
    //        query = query.Where(a => a.Category.ToLower() == queryParameters.category.ToLower());
    //    }

    //    //if (queryParameters.page > 0 && queryParameters.pageSize > 0)
    //    //{
    //    //    var skip = (queryParameters.page - 1) * queryParameters.pageSize;
    //    //    query = query.Skip(skip).Take(queryParameters.pageSize);
    //    //}


    //    query = queryParameters.order.ToLower() switch
    //    {
    //        "price_asc" => query.OrderBy(a => a.Price),
    //        "price_desc" => query.OrderByDescending(a => a.Price),
    //        "rating_desc" => query.OrderByDescending(a => a.AverageRate),
    //        "rating_asc" => query.OrderBy(a => a.AverageRate),
    //        "latest_asc" => query.OrderBy(a=>a.Created),
    //        "latest_desc" => query.OrderByDescending(a => a.Created),
    //        _ => query
    //    };

    //    var dtoQuery =  query.Select(p => new ProductOverviewDto
    //    {
    //        Id = p.Id,
    //        Name = p.Name,
    //        Price = p.Price,
    //        PriceAfterDiscount = p.PriceAfterDiscount,
    //        PageNumber = currentPage,
    //        ImageUrl = p.Images.FirstOrDefault() != null ? p.Images.FirstOrDefault().RelativePath : null,
    //        TotalRatingPeople = p.TotalRatePeople,
    //        AverageRatingPeople = p.AverageRate,
    //        IsInWishList = wishlistIds.Contains(p.Id),
    //    });

    //    return await dtoQuery.ToPagedListAsync(queryParameters.page, queryParameters.pageSize, categoryNameDisplay);
    //}
    public async Task<PagedResult<ProductOverviewDto>> GetProductsOverview(QueryParameters queryParameters, string? userId = null)
    {
       
        int currentPage = queryParameters.page > 0 ? queryParameters.page : 1;
        int pageSize = queryParameters.pageSize > 0 ? queryParameters.pageSize : 12;

        var query = ctx.Products.AsQueryable();
        query.Include(c => c.CategoryImage);
        if(queryParameters.search != null)
        {
            if (queryParameters.LanguageCode == "ar")
            {
                query = query.Where(a => a.Name.Contains(queryParameters.search) || a.Descriptions.Contains(queryParameters.search));
            }
            else
            {
                var translatedProductIds = await ctx.ProductTranslations
                    .Where(t => t.Name.Contains(queryParameters.search) || t.Description.Contains(queryParameters.search))
                    .Select(t => t.ProductId)
                    .ToListAsync();
                query = query.Where(a => translatedProductIds.Contains(a.Id));
            }
       
        }
        string categoryNameDisplay = "All Categories";
        if (!string.IsNullOrEmpty(queryParameters.category))
        {
            var cat = queryParameters.category.ToLower();
            query = query.Where(a => a.CategoryImage.CategoryValue.ToLower() == cat);
            var category= await ctx.CategoryImages.FirstOrDefaultAsync(c => c.CategoryValue.ToLower() == cat);
            if (category != null)
            {
                categoryNameDisplay = queryParameters.LanguageCode == "en" ? category.CategoryValue.Replace("-", " ") : category.Category.Replace("-", " ");

            }
            else
            {
                categoryNameDisplay = queryParameters.category;
            }
        }

        query = (queryParameters.order ?? "").ToLower() switch
        {
            "price_asc" => query.OrderBy(a => a.Price),
            "price_desc" => query.OrderByDescending(a => a.Price),
            "rating_desc" => query.OrderByDescending(a => a.AverageRate),
            "rating_asc" => query.OrderBy(a => a.AverageRate),
            "latest_asc" => query.OrderBy(a => a.Created),
            "latest_desc" => query.OrderByDescending(a => a.Created),
            _ => query.OrderBy(a => a.Id) 
        };

        var wishlistIds = await GetWishlistIdsAsync(userId, queryParameters.Ids);

        var dtoQuery = query.Select(p => new ProductOverviewDto
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            PriceAfterDiscount = p.PriceAfterDiscount,
            
            PageNumber = currentPage, 

            ImageUrl = p.Images.FirstOrDefault() != null ? p.Images.FirstOrDefault().RelativePath : null,

            TotalPeopleRating = p.TotalRatePeople,
            AverageRatingPeople = p.AverageRate,
            IsInWishList = wishlistIds.Contains(p.Id),
        });

       
        return await dtoQuery.ToPagedListAsync(currentPage, pageSize, categoryNameDisplay);
    }
    public async Task<PagedResult<ProductTranslationOverviewDto>> GetProductsTranslationOverview(QueryParameters queryParameters, string? userId = null)
    {
        int currentPage = queryParameters.page > 0 ? queryParameters.page : 1;
        int pageSize = queryParameters.pageSize > 0 ? queryParameters.pageSize : 12;

        var query = ctx.ProductTranslations.AsQueryable();
        query.Include(t => t.Product).ThenInclude(c => c.CategoryImage);
        if (queryParameters.search != null)
        {
            if (queryParameters.LanguageCode == "ar")
            {
                query = query.Where(a => a.Name.Contains(queryParameters.search) || a.Description.Contains(queryParameters.search));
            }
            else
            {
                var translatedProductIds = await ctx.ProductTranslations
                    .Where(t => t.Name.Contains(queryParameters.search) || t.Description.Contains(queryParameters.search))
                    .Select(t => t.ProductId)
                    .ToListAsync();
                query = query.Where(a => translatedProductIds.Contains(a.Id));
            }

        }
        string categoryNameDisplay = "All Categories";
        //if (!string.IsNullOrEmpty(queryParameters.category))
        //{
        //    var cat = queryParameters.category.ToLower();
        //    query = query.Where(a => a.Category.ToLower() == cat);
        //    categoryNameDisplay = queryParameters.category;
        //}
        if (!string.IsNullOrEmpty(queryParameters.category))
        {
            var cat = queryParameters.category.ToLower();
            query = query.Where(a => a.Product.CategoryImage.CategoryValue.ToLower() == cat);
           // query = query.Where(a => a.Category.ToLower() == cat);
            var category = await ctx.CategoryImages.FirstOrDefaultAsync(c => c.CategoryValue.ToLower() == cat);
            if (category != null)
            {
                categoryNameDisplay = queryParameters.LanguageCode == "en" ? category.CategoryValue.Replace("-", " ") : category.Category.Replace("-", " ");

            }
            else
            {
                categoryNameDisplay = queryParameters.category;
            }
        }
        query = (queryParameters.order ?? "").ToLower() switch
        {
            "price_asc" => query.OrderBy(a => a.Product.Price),
            "price_desc" => query.OrderByDescending(a => a.Product.Price),
            "rating_desc" => query.OrderByDescending(a => a.Product.AverageRate),
            "rating_asc" => query.OrderBy(a => a.Product.AverageRate),
            "latest_asc" => query.OrderBy(a => a.Product.Created),  
            "latest_desc" => query.OrderByDescending(a => a.Product.Created),
            _ => query.OrderBy(a => a.Id) 
        };

        var wishlistIds = await GetWishlistIdsAsync(userId, queryParameters.Ids);

        var dtoQuery = query.Select(p => new ProductTranslationOverviewDto
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Product.Price,
            PriceAfterDiscount = p.Product.PriceAfterDiscount,

            PageNumber = currentPage,

            ImageUrl = p.Product.Images.FirstOrDefault() != null ? p.Product.Images.FirstOrDefault().RelativePath : null,

            TotalPeopleRating = p.Product.TotalRatePeople,
            AverageRatingPeople = p.Product.AverageRate,
            IsInWishList = wishlistIds.Contains(p.Id) 
        });

        return await dtoQuery.ToPagedListAsync(currentPage, pageSize, categoryNameDisplay);
    }
    //public async Task<PagedResult<ProductTranslationOverviewDto>> GetProductsTranslationOverview(QueryParameters queryParameters, string? userId = null)
    //{
    //   // queryParameters.page = queryParameters.page > 0 ? queryParameters.page   :  1;
    //    queryParameters.pageSize = queryParameters.pageSize > 0 ? queryParameters.pageSize : 12;


    //    var query = ctx
    //        .ProductTranslations
    //        .Include(t => t.Product)            
    //        .ThenInclude(p => p.Variants)   
    //        .Include(p=>p.Product)
    //        .ThenInclude(t=>t.Images)
    //        .AsQueryable();
    //    string categoryNameDisplay = "All Categories";
    //    if (!string.IsNullOrEmpty(queryParameters.category))
    //    {
    //        var cat = queryParameters.category.ToLower();
    //        query = query.Where(a => a.Category.ToLower() == cat);
    //        categoryNameDisplay = queryParameters.category;
    //    }
    //    int currentPage = queryParameters.page > 0 ? queryParameters.page : 1;
    //    var wishlistIds = await GetWishlistIdsAsync(userId, queryParameters.Ids);

    //    //if (!string.IsNullOrEmpty(queryParameters.category))
    //    //{
    //    //    query = query.Where(a => a.Category.ToLower() == queryParameters.category.ToLower());

    //    //}

    //    //if (queryParameters.page > 0 && queryParameters.pageSize > 0)
    //    //{
    //    //    var skip = (queryParameters.page - 1) * queryParameters.pageSize;
    //    //    query = query.Skip(skip).Take(queryParameters.pageSize);
    //    //}


    //    query = queryParameters.order.ToLower() switch
    //    {
    //        "price_asc" => query.OrderBy(a => a.Price),
    //        "price_desc" => query.OrderByDescending(a => a.Price),
    //        "rating_desc" => query.OrderByDescending(a => a.AverageRate),
    //        "rating_asc" => query.OrderBy(a => a.AverageRate),
    //        _ => query
    //    };


    //    var dtoQuery =  query.Select(p => 
    //        new ProductTranslationOverviewDto
    //    {
    //        Id = p.Id,
    //        Name = p.Name,
    //        Price = p.Price,
    //        PriceAfterDiscount = p.PriceAfterDiscount,
    //        PageNumber = currentPage,
    //            ImageUrl = p.Product.Images.FirstOrDefault().RelativePath,
    //        TotalPeopleRating = p.Product.TotalRatePeople,
    //        AverageRatingPeople = p.Product.AverageRate,
    //         IsInWishList = wishlistIds.Contains(p.Id)
    //        });

    //    return await dtoQuery.ToPagedListAsync(queryParameters.page, queryParameters.pageSize, categoryNameDisplay);
    //}

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
                TotalPeopleRating = p.TotalRatePeople,
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
                TotalPeopleRating = p.TotalRatePeople,
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
            .Select(t => t.Product.CategoryImage.Category)
            .Distinct()
            .ToList();
    }

    public async Task<PagedResult<ProductOverviewDto>> GetProductsByCategory(QueryParameters queryParameters , int Id, string? userId = null)
    {
        int currentPage = queryParameters.page > 0 ? queryParameters.page : 1;
        int pageSize = queryParameters.pageSize > 0 ? queryParameters.pageSize : 12;
        var wishlistIds = await GetWishlistIdsAsync(userId, queryParameters.Ids);
       // var product=ctx.Products.Include(c=>c.CategoryImage).FirstOrDefault(a => a.Id == Id);
        var product=ctx.Products.FirstOrDefault(a => a.Id == Id);
        var categ = ctx.CategoryImages.FirstOrDefault(c => c.Id == product.CategoryFK);
        if (product == null) 
            {
            return new PagedResult<ProductOverviewDto>();
        }
        var dtoQuery =ctx.Products.Include(a => a.CategoryImage)
            .Where(a => a.CategoryImage.CategoryValue.ToLower() == categ.CategoryValue.ToLower() && a.Id != Id)
            .Include(a => a.Images)
            .Take(4)
            .Select(p => new ProductOverviewDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                PriceAfterDiscount = p.PriceAfterDiscount,
                ImageUrl = p.Images.FirstOrDefault().RelativePath,
                TotalPeopleRating = p.TotalRatePeople,
                AverageRatingPeople = p.AverageRate,
                IsInWishList = wishlistIds.Contains(p.Id)
            }).AsQueryable();
        string categoryNameDisplay =  product.CategoryImage.Category.Replace("-", " ");
        return await dtoQuery.ToPagedListAsync(currentPage, pageSize, categoryNameDisplay);

    }


    public async Task<PagedResult<ProductTranslationOverviewDto>> GetProductTranslationsByCategory(QueryParameters queryParameters, int Id, string? userId = null)
    {
        int currentPage = queryParameters.page > 0 ? queryParameters.page : 1;
        int pageSize = queryParameters.pageSize > 0 ? queryParameters.pageSize : 12;
        var wishlistIds = await GetWishlistIdsAsync(userId, queryParameters.Ids);
        //  var product = ctx.Products.Include(c => c.CategoryImage).FirstOrDefault(a => a.Id == Id);
        var product = ctx.Products.FirstOrDefault(a => a.Id == Id);
        var categ = ctx.CategoryImages.FirstOrDefault(c => c.Id == product.CategoryFK);

        if (product == null)
        {
            return new PagedResult<ProductTranslationOverviewDto>();
        }
        var dtoQuery = ctx.ProductTranslations.Include(a => a.Product).ThenInclude(c => c.CategoryImage)
            .Where(a => a.Product.CategoryImage.CategoryValue.ToLower() == categ.CategoryValue.ToLower() && a.Id != Id)
            .Include(a=>a.Product)
            .ThenInclude(a => a.Images)
            .Take(4)
            .Select(pt => new ProductTranslationOverviewDto(){
                Id = pt.Id,
                Name = pt.Name,
                Price = pt.Product.Price,
                ImageUrl = pt.Product.Images.FirstOrDefault().RelativePath,

                PriceAfterDiscount = pt.Product.PriceAfterDiscount,
                TotalPeopleRating = pt.Product.TotalRatePeople,
                AverageRatingPeople = pt.Product.AverageRate,
                IsInWishList = wishlistIds.Contains(pt.Id)
            }).AsQueryable();


        string categoryNameDisplay = product.CategoryImage.CategoryValue.Replace("-", " ");
        return await dtoQuery.ToPagedListAsync(currentPage, pageSize, categoryNameDisplay);
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
          
            var category = ctx.CategoryImages.FirstOrDefault(c => c.CategoryValue.ToLower() == productDto.CategoryValue.ToLower());
            var product = new Product
            {
                Name = productDto.Name,
                FullDescription = productDto.Description,
                PriceAfterDiscount = productDto.PriceBeforeDiscount != null
                    ? productDto.PriceBeforeDiscount - productDto.Price
                    : 0,
                Price = productDto.Price,
              //  Category = productDto.Category,
              CategoryFK= category != null ? category.Id : 0,
              
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

