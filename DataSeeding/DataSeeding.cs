using System.Linq.Expressions;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using WallsShop.Context;
using WallsShop.Entity;

public static class TranslationSeeder
{
    public static async Task SeedEnglishTranslationsAsync(WallShopContext context, string jsonFilePath)
    {
        try
        {
            if (!File.Exists(jsonFilePath)) return;
            
            var jsonData = await File.ReadAllTextAsync(jsonFilePath);
            var englishProducts = JsonSerializer.Deserialize<List<EnglishProductDto>>(jsonData);

            if (englishProducts == null) return;

            foreach (var item in englishProducts)
            {
                var product = await context
                    .ProductTranslations
                    .FirstOrDefaultAsync(p => p.Name == item.Name);
                if (product != null)
                {
                    product.Category = item.Category;
                    product.SKU = item.SKU;
                    product.AverageRate = 0;
                    context.ProductTranslations.Update(product);
                    await context.SaveChangesAsync();
                }
            }

        }
        catch (Exception e)
        {
            throw new Exception("Error during seeding English translations", e);
        }
    }

    public static async Task SeedProductColorsAsync(WallShopContext _context, string jsonFilePath)
    {
        var jsonData = await File.ReadAllTextAsync(jsonFilePath);
        var englishProducts = JsonSerializer.Deserialize<List<EnglishProductDto>>(jsonData);
        foreach (var item in englishProducts)
        {
            // 1. Find the product ID using the SKU
            var product = await _context
                .Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.SKU == item.SKU);

            if (product != null && item.Color != null)
            {
                foreach (var colorName in item.Color)
                {
                    // 2. Check if this specific color/product combo already exists to avoid duplicates
                    bool exists = await _context.Colors.AnyAsync(pc =>
                        pc.ProductId == product.Id &&
                        pc.Color == colorName &&
                        pc.LanguageCode == "en");

                    if (!exists)
                    {
                        var newColor = new ProductColor
                        {
                            ProductId = product.Id,
                            Color = colorName,
                            LanguageCode = "en" // Hardcoded as per your requirement
                        };

                        _context.Colors.Add(newColor);
                    }
                }
            }
        }

        // 3. Save all new color entries at once for better performance
        await _context.SaveChangesAsync();
    }


    public static async Task SeedProductVariantsAsync(WallShopContext _context, string jsonFilePath)
    {
        try
        {
            var jsonDataText = await File.ReadAllTextAsync(jsonFilePath);
            var jsonData = JsonSerializer.Deserialize<List<VariantJsonModel>>(jsonDataText);

            if (jsonData == null) return;

            foreach (var item in jsonData)
            {

                var translation = await 
                    _context
                    .ProductTranslations
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.Name == item.Title);
                
                if (translation == null)
                    continue;

                var SKU = _context.Products
                    .AsNoTracking()
                    .FirstOrDefault(p => p.Id == translation.ProductId)?.SKU;
                 
                    foreach (var v in item.Variants)
                    {

                        var newVariant = new Variant
                        {
                            ProductId = translation.ProductId,  
                            SKU = SKU ?? "NO SKU",
                            Size = v.Size,
                            Type = v.Type ?? "",
                            Price = decimal.TryParse(v.CurrentPriceEGP, out var p) ? p : 0,
                            PriceBeforeDiscount = decimal.TryParse(v.PriceBeforeDiscountEGP, out var pbd)
                                ? pbd
                                : (decimal?)null,
                            DiscountRate = decimal.TryParse(v.DiscountRatePercent, out var dr) ? dr : 0,
                            LanguageCode = "en"
                        };

                        _context.Variants.Add(newVariant);
                    }

                await _context.SaveChangesAsync();

            }
        }
        catch (Exception e)
        {
            throw new Exception("Error during seeding product variants", e);
        }
    }
    }



    public class EnglishProductDto
    {
        public string Name { get; set; }
        public List<string> Description { get; set; }
        public string SKU { get; set; }
        
        public string Category { get; set; }

        public List<string> Color { get; set; }
    }

    public class VariantJsonModel
    {
        public string Title { get; set; }
        public string Handle { get; set; }
        public string Url { get; set; }
        public List<VariantItem> Variants { get; set; }
        public List<string> Images { get; set; }
    }

    public class VariantItem
    {
        public string Size { get; set; }
        public string Type { get; set; }

        // Using string because JSON prices are quoted (e.g., "12711.00")
        public string CurrentPriceEGP { get; set; }
        public string PriceBeforeDiscountEGP { get; set; }
        public string DiscountRatePercent { get; set; }
        public string SKU { get; set; }
    }
