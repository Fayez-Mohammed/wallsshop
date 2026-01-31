using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Threading.Tasks;
using WallsShop.Context;
using WallsShop.DTO;

namespace WallsShop.Services;

public class CartService  
{ 
    private readonly ConcurrentDictionary<string, CachedCart> _carts = new();

    private readonly IServiceProvider _serviceProvider;
 //   private readonly IDistributedCache _cache;
    public CartService(IServiceProvider serviceProvider)//, IDistributedCache cache
    {
        _serviceProvider = serviceProvider;
    //    _cache = cache;
    }
    public async Task AddToCart(string userId, CartItem item)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<WallShopContext>();

            var product = await context.Products
                .Include(p => p.Variants)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == item.ProductId);

            if (product == null)
                throw new Exception("Product does not exist");

            // Try to find variant (optional)
            var variant = product.Variants?
                .FirstOrDefault(v => v.Id == item.VariantId);

            // If VariantId is provided but not found → error
            if (item.VariantId != 0 && variant == null)
                throw new Exception("Variant does not exist for this product");

            // Fill cart item safely
            item.ProductName = product.Name;
            item.ImageUrl = product.Images?.FirstOrDefault()?.RelativePath ?? string.Empty;

            item.UnitPrice = variant?.Price > 0
                ? variant.Price
                : product.Price;

            item.Size = variant?.Size ?? string.Empty;
            item.Type = variant?.Type ?? string.Empty;
            item.Color = item.Color ?? string.Empty;

            // Add to cached cart
            var cachedCart = _carts.GetOrAdd(userId, _ => new CachedCart());

            var existingItem = cachedCart.Cart.Items
                .FirstOrDefault(i =>
                    i.ProductId == item.ProductId &&
                    i.VariantId == item.VariantId &&
                    i.Color == item.Color);

            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                cachedCart.Cart.Items.Add(item);
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error adding to cart: {ex.Message}", ex);
        }
    }

    //public List<CartItem> GetCartItems(string userId)
    //{
    //    if (_carts.TryGetValue(userId, out var cached))
    //    {
    //        return cached.Cart.Items;
    //    }

    //    return new List<CartItem>();
    //}
    public async Task<List<CartItem>> GetCartItems(string userId, string languageCode="ar")
    {
        if (_carts.TryGetValue(userId, out var cached))
        {

         var items = await GetProductImageName(cached.Cart.Items, languageCode);
            return items;


        }

        return new List<CartItem>();
    }
    public async Task<List<CartItem>> GetProductImageName( List<CartItem> items, string languageCode)
    {
        List<CartItem> returnList = new List<CartItem>();
        using (var scope = _serviceProvider.CreateScope())
        {
            
            var context = scope.ServiceProvider.GetRequiredService<WallShopContext>();
            if (languageCode.ToLower() == "ar")
            {
                foreach (var item in items)
                {
                    var product = context
                        .Products
                        .Include(a => a.Images)
                        .Include(a => a.Variants)
                        .Where(a => a.Id == item.ProductId)
                        .Select(a => new CartItem()
                        {
                            ProductId = item.ProductId,
                            ProductName = a.Name,
                            VariantId = item.VariantId,
                            Quantity = item.Quantity,
                            Color = item.Color,
                            Size = a.Variants.FirstOrDefault(b => b.Id == item.VariantId).Size ?? "",
                            Type = a.Variants.FirstOrDefault(b => b.Id == item.VariantId).Type ?? "",
                            UnitPrice = item.VariantId > 0
                            ? (a.Variants.FirstOrDefault(b => b.Id == item.VariantId).Price)
                            : a.PriceAfterDiscount,
                            OriginalPrice = item.VariantId > 0
                             ? (decimal)(a.Variants.FirstOrDefault(b => b.Id == item.VariantId).PriceBeforeDiscount ?? a.Variants.FirstOrDefault(b => b.Id == item.VariantId).Price)
                             : a.Price,
                            //UnitPrice = a.Variants.Where(b => b.Id == item.VariantId).FirstOrDefault().Price > 0 ?
                            //    a.Variants.Where(b => b.Id == item.VariantId).FirstOrDefault().Price : a.Price,
                            ImageUrl = a.Images.FirstOrDefault().RelativePath ?? string.Empty,
                        }).FirstOrDefault();
                    if (product == null) throw new Exception("Product does not exist");
                    returnList.Add(product);
                }

            }
            else if (languageCode == "en")
            {
                foreach (var item in items)
                {
                    var product = context.ProductTranslations.Include(t => t.Product).ThenInclude(p => p.Variants)

                        .Where(t => t.ProductId == item.ProductId)
                        .Select(t => new CartItem()
                        {
                            ProductId = item.ProductId,

                            ProductName = t.Name,

                            VariantId = item.VariantId,

                            Size = t.Product.Variants
                                    .Where(v => v.Id == item.VariantId)
                                    .Select(v => v.Size)
                                    .FirstOrDefault() ?? "",

                            Type = t.Product.Variants
                                    .Where(v => v.Id == item.VariantId)
                                    .Select(v => v.Type)
                                    .FirstOrDefault() ?? "",

                            Color = item.Color,
                            Quantity = item.Quantity,
                            UnitPrice = item.VariantId > 0
                            ? (t.Product.Variants.FirstOrDefault(v => v.Id == item.VariantId).Price)
                            : t.Product.PriceAfterDiscount,

                            OriginalPrice = item.VariantId > 0
                            ? (decimal)(t.Product.Variants.FirstOrDefault(v => v.Id == item.VariantId).PriceBeforeDiscount ?? t.Product.Variants.FirstOrDefault(v => v.Id == item.VariantId).Price)
                            : t.Product.Price,
                            //UnitPrice = t.Product.Variants
                            //            .Where(v => v.Id == item.VariantId)
                            //            .Select(v => v.Price > 0 ? v.Price : t.Product.Price)
                            //            .FirstOrDefault(),


                            ImageUrl = t.Product.Images.FirstOrDefault().RelativePath ?? string.Empty,
                        })
                        .FirstOrDefault();

                    if (product == null)
                    {
                        var productar = context
                      .Products
                      .Include(a => a.Images)
                      .Include(a => a.Variants)
                      .Where(a => a.Id == item.ProductId)
                      .Select(a => new CartItem()
                      {
                          ProductId = item.ProductId,
                          ProductName = a.Name,
                          VariantId = item.VariantId,
                          Quantity = item.Quantity,
                          Color = item.Color,
                          Size = a.Variants.Where(b => b.Id == item.VariantId).FirstOrDefault().Size ?? "",
                          Type = a.Variants.Where(b => b.Id == item.VariantId).FirstOrDefault().Type ?? "",
                          UnitPrice = a.Variants.Where(b => b.Id == item.VariantId).FirstOrDefault().Price > 0 ?
                              a.Variants.Where(b => b.Id == item.VariantId).FirstOrDefault().Price : a.Price,
                          ImageUrl = a.Images.FirstOrDefault().RelativePath ?? string.Empty,
                      }).FirstOrDefault();
                        if (productar == null) throw new Exception("Product does not exist");
                        returnList.Add(productar);
                    }
                    else
                    {
                        returnList.Add(product);
                    }
                }
            }
        }

        return returnList;
    }

    public void CleanupExpiredCarts()
    {
        var expirationTime = DateTime.UtcNow.AddHours(-48);
        var expiredKeys = _carts.Where(kvp => kvp.Value.CreatedAt < expirationTime)
            .Select(kvp => kvp.Key);

        foreach (var key in expiredKeys)
        {
            _carts.TryRemove(key, out _);
        }
    }
    // ضيف الدالة دي جوه CartService.cs
    public void ClearCart(string userId)
    {
        // TryRemove بتحذف العنصر من الـ Dictionary بأمان
        _carts.TryRemove(userId, out _);
    }

    // داخل CartService.cs

    public async Task<List<CartItem>?> UpdateQuantityAsync(string userId, int productId, int variantId, int quantity, string color, string languageCode)
    {
      
        if (_carts.TryGetValue(userId, out var cachedCart))
        {
           
            var itemToUpdate = cachedCart.Cart.Items
                .FirstOrDefault(i => i.ProductId == productId &&
                                     i.VariantId == variantId &&
                                     i.Color == (color ?? ""));

            if (itemToUpdate != null)
            {
                if (quantity <= 0)
                {
                    cachedCart.Cart.Items.Remove(itemToUpdate);
                }
                else
                {
                    itemToUpdate.Quantity = quantity;
                }

                return await GetCartItems(userId, languageCode);
            }
        }

        return null;
    }
    // داخل CartService.cs

    public async Task<List<CartItem>?> RemoveItemAsync(string userId, int productId, int variantId, string color, string languageCode)
    {
        if (_carts.TryGetValue(userId, out var cachedCart))
        {
            var itemToDelete = cachedCart.Cart.Items
                .FirstOrDefault(i => i.ProductId == productId &&
                                     i.VariantId == variantId &&
                                     i.Color == (color ?? ""));

            if (itemToDelete != null)
            {
                cachedCart.Cart.Items.Remove(itemToDelete);

                return await GetCartItems(userId, languageCode);
            }
        }

        return null;
    }
}