//using Microsoft.AspNetCore.Identity;
//using System.Security.Claims;
//using WallsShop.Entity;

//namespace WallsShop.Helpers
//{
//    public class BlockedUserMiddleware
//    {
//        private readonly RequestDelegate _next;

//        public BlockedUserMiddleware(RequestDelegate next)
//        {
//            _next = next;
//        }

//        public async Task InvokeAsync(
//            HttpContext context,
//            UserManager<User> userManager)
//        {
//            // لو مش Authenticated → عدّي
//            if (!context.User.Identity?.IsAuthenticated ?? true)
//            {
//                await _next(context);
//                return;
//            }

//            // لو Authenticated → نراجع
//            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

//            if (userId != null)
//            {
//                var user = await userManager.FindByIdAsync(userId);

//                if (user?.IsBlocked == true)
//                {
//                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
//                    return;
//                }
//            }

//            await _next(context);
//        }
//    }

//}

