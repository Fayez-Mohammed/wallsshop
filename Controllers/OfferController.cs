using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WallsShop.DTO;
using WallsShop.Entity;
using WallsShop.Repository;
using WallsShop.Services;

namespace WallsShop.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OffersController(
    OfferRepository repo,
    IEmailService emailService,
    IServiceScopeFactory scopeFactory,
    UserManager<User> userManager
    ) : ControllerBase
{
    [Authorize("Admin")]

    [HttpPost("add-offer")]
   public async Task<IActionResult> AddOffer([FromForm] AddOfferDto model)
   {
      var result = await repo.AddOffer( model);
        if (result)
        {
            string subject = string.Empty;
            string htmlMessage = string.Empty;

            // تصميم ثابت (CSS) عشان نستخدمه في اللغتين ونقلل تكرار الكود
            // الـ CSS ده بيضمن شكل "الكارد" والزرار والألوان
            string commonStyle = @"
        body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f6f6f6; margin: 0; padding: 0; }
        .container { max-width: 600px; margin: 20px auto; background-color: #ffffff; border-radius: 10px; box-shadow: 0 4px 15px rgba(0,0,0,0.1); overflow: hidden; }
        .header { background-color: #2c3e50; padding: 20px; text-align: center; }
        .header h1 { color: #ffffff; margin: 0; font-size: 24px; letter-spacing: 1px; }
        .content { padding: 30px; }
        .title { color: #e74c3c; font-size: 24px; font-weight: bold; margin-bottom: 15px; }
        .desc { color: #555555; font-size: 16px; line-height: 1.6; margin-bottom: 25px; }
        .date-badge { background-color: #fff3cd; color: #856404; padding: 12px; border-radius: 6px; display: inline-block; font-weight: bold; border: 1px solid #ffeeba; }
        .btn-wrap { text-align: center; margin-top: 30px; }
        .btn { background-color: #e74c3c; color: #ffffff !important; padding: 14px 30px; text-decoration: none; border-radius: 50px; font-weight: bold; font-size: 16px; display: inline-block; box-shadow: 0 4px 6px rgba(231, 76, 60, 0.3); }
        .footer { background-color: #eeeeee; padding: 15px; text-align: center; font-size: 12px; color: #999999; }
    ";

        //    if (model.languageCode == "en")
        //    {
        //        subject = $"🔥 Hot Offer: {model.Name}";
        //        htmlMessage = $@"
        //<!DOCTYPE html>
        //<html lang='en'>
        //<head><style>{commonStyle}</style></head>
        //<body>
        //    <div class='container'>
        //        <div class='header'>
        //            <h1>Walls Shop</h1>
        //        </div>
        //        <div class='content' style='text-align: left; direction: ltr;'>
        //            <div class='title'>{model.Name}</div>
        //            <div class='desc'>{model.Description}</div>
                    
        //            <div class='date-badge'>
        //                ⏳ Ends on: {model.EndDate:yyyy-MM-dd}
        //            </div>

        //            <div class='btn-wrap'>
        //                <a href='http://your-website.com/offers' class='btn'>Shop Now</a>
        //            </div>
        //        </div>
        //        <div class='footer'>
        //            <p>© {DateTime.Now.Year} Walls Shop. All rights reserved.</p>
        //        </div>
        //    </div>
        //</body>
        //</html>";
        //    }
        //    else
        //    {
                subject = $"🔥 عرض جديد: {model.ArabicName}";
                htmlMessage = $@"
        <!DOCTYPE html>
        <html lang='ar'>
        <head>
            <style>
                @import url('https://fonts.googleapis.com/css2?family=Cairo:wght@400;700&display=swap');
                {commonStyle}
                body, .container {{ font-family: 'Cairo', Tahoma, sans-serif; }}
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='header'>
                    <h1>Walls Shop</h1>
                </div>
                <div class='content' style='text-align: right; direction: rtl;'>
                    <div class='title'>{model.ArabicName}</div>
                    <div class='desc'>{model.ArabicDescription}</div>
                    
                    <div class='date-badge'>
                        ⏳ ينتهي العرض في: {model.EndDate:yyyy-MM-dd}
                    </div>

                    <div class='btn-wrap'>
                        <a href='http://your-website.com/offers' class='btn'>تسوق الآن</a>
                    </div>
                </div>
                <div class='footer'>
                    <p>نتمنى لك تجربة تسوق ممتعة</p>
                    <p>© {DateTime.Now.Year} Walls Shop. جميع الحقوق محفوظة.</p>
                </div>
            </div>
        </body>
        </html>";
         //   }
            _ = Task.Run(async () =>
            {
             
                using var scope = scopeFactory.CreateScope();

                var scopedUserManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                var scopedEmailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                try
                {
                    var usersEmails = await scopedUserManager.Users
                                            .Where(u => u.Email != null)
                                            .Select(u => u.Email)
                                            .ToListAsync();

                    foreach (var email in usersEmails)
                    {
                        try
                        {
                            await scopedEmailService.SendEmailAsync(email!, subject, htmlMessage);
                        }
                        catch { /* Ignored */ }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });

            return Ok(new { response = "Offer added successfully" });
        }

        return BadRequest("Failed to add offer");
     }
    


   [HttpGet("get-offer")]
   public async Task<IActionResult> GetOffers([FromQuery]  string languageCode)
   {
       var offers = await repo.GetOffers(languageCode);

       return Ok(new {reponse = offers});
   }
    [Authorize("Admin")]

    [HttpDelete("delete-offer")]
    public async Task<IActionResult> DeleteOffer([FromQuery] int offerId)
    {
        var result = await repo.DeleteOffer(offerId);
        if (result)
            return Ok(new {response = "Offer was deleted successfully"});
        return BadRequest();
    }
}