using Microsoft.EntityFrameworkCore;
using WallsShop.Context;
using WallsShop.DTO;
using WallsShop.Entity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace WallsShop.Repository;

public class ReviewRepository(WallShopContext ctx)
{
    [Authorize]
    public async  Task<bool> CreateReview(ReviewDto review , ClaimsPrincipal  User,  UserManager<User> _userManager)
    {
        var user = await _userManager.GetUserAsync(User);
        
        var product = ctx.Products.Find(review.ProductId);
        if (product ==null)
            return false;
        try
        {
            product.TotalRatePeople += 1;
            product.RatingSum += review.Rating;
            if (product.TotalRatePeople > 0)
            {
                product.AverageRate = product.RatingSum / product.TotalRatePeople;
            }
            else
            {
                product.AverageRate = 0;
            }
            Review newReview = new Review()
            {
                ProductId = review.ProductId,
                Comment = review.Comment,
                UserName =   user.Name,
                ReviewDate = DateTime.Now,
                Rate = review.Rating 
            };
            ctx.Reviews.Add(newReview);
            await ctx.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public async  Task<bool> DeleteReview(int id)
    {
        var review = await ctx.Reviews.FindAsync(id);
            
        if (review == null)
            return false;
        try
        {
            var user =await  ctx.Products.Where(a => a.Id == review.ProductId).FirstOrDefaultAsync();


            if (user == null)
                return false;
            
            user.TotalRatePeople -= 1;
            user.RatingSum -= review.Rate;
            user.AverageRate = user.RatingSum / (user.TotalRatePeople > 0 ? user.TotalRatePeople:1);
            
            ctx.Reviews.Remove(review);
            await ctx.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
    // 3.2 out of 132 person 

    // fayez : 

    // adham : 
    //public async Task<ReviewResponseDto> GetProductReviews(int productId , string userId , bool isAdmin  )
    //{
    //    return await  ctx
    //        .Products
    //        .Include(a=>a.Reviews)
    //        .Where(a=>a.Id == productId)?.
    //        Select(a=> new ReviewResponseDto()
    //        {
    //            ProductId = a.Id,
    //            AverageRating = a.AverageRate,
    //            TotalReviews = a.TotalRatePeople,
    //            SingleReviews = a.Reviews.Select(a=>new SingleReviewDto()
    //            {
    //                UserName = a.UserName,
    //                Comment = a.Comment,
    //                Date = a.ReviewDate,
    //                CanBeDeleted = userId == a.UserName  || isAdmin ? true:false,
    //                Rate = a.Rate
    //            }).ToList()
    //        }).FirstOrDefaultAsync();
    //}
    public async Task<ReviewResponseDto> GetProductReviews(int productId, string userId, bool isAdmin)
    {
        return await ctx
            .Products
            .Include(a => a.Reviews)
            .Where(a => a.Id == productId)
            .Select(a => new ReviewResponseDto()
            {
                ProductId = a.Id,
                AverageRating = a.AverageRate,
                TotalReviews = a.TotalRatePeople,
                SingleReviews = a.Reviews.Select(r => new SingleReviewDto()
                {
                    UserName = r.UserName,
                    Comment = r.Comment,
                    Date = r.ReviewDate,
                    CanBeDeleted = userId == r.UserName || isAdmin, // Removed ternary
                    Rate = r.Rate
                }).ToList()
            })
            .FirstOrDefaultAsync();
    }
}