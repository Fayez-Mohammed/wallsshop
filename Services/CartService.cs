using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using WallsShop.Context;
using WallsShop.DTO;

namespace WallsShop.Services;

public class CartService  
{ 
    private readonly ConcurrentDictionary<string, CachedCart> _carts = new();

    private readonly IServiceProvider _serviceProvider;

    public CartService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
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

    public List<CartItem> GetCartItems(string userId)
    {
        if (_carts.TryGetValue(userId, out var cached))
        {
            return cached.Cart.Items;
        }

        return new List<CartItem>();
    }

    public async Task<List<CartItem>> GetProductImageName( List<CartItem> items)
    {
        List<CartItem> returnList = new List<CartItem>();
        using (var scope = _serviceProvider.CreateScope())
        {
            
            var context = scope.ServiceProvider.GetRequiredService<WallShopContext>();
           
            foreach (var item in items)
            {
                var product =   context
                    .Products
                    .Include(a => a.Images)
                    .Include(a => a.Variants)
                    .Where(a=>a.Id == item.ProductId)
                    .Select(a => new CartItem()
                    {
                        ProductId = item.ProductId,
                        ProductName = a.Name,
                        VariantId = item.VariantId,
                        Size = a.Variants.Where(b=>b.Id == item.VariantId).FirstOrDefault().Size ?? "",
                        Type = a.Variants.Where(b=>b.Id == item.VariantId).FirstOrDefault().Type ?? "",
                        UnitPrice = a.Variants.Where(b=>b.Id == item.VariantId).FirstOrDefault().Price > 0 ?
                            a.Variants.Where(b=>b.Id == item.VariantId).FirstOrDefault().Price : a.Price,
                        ImageUrl = a.Images.FirstOrDefault().RelativePath ?? string.Empty,
                    }).FirstOrDefault();
                if (product == null) throw new Exception("Product does not exist");
                returnList.Add(product);
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
}