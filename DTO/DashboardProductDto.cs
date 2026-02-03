namespace WallsShop.DTO;

// DTO لعرض جميع بيانات المنتج في Dashboard (بدون فلتر لغة)
public class DashboardProductDto
{
    public int Id { get; set; }
    
    // الأسماء
    public string NameAR { get; set; }
    public string NameEN { get; set; }
    
    // الأوصاف
    public string DescriptionAR { get; set; }
    public string DescriptionEN { get; set; }
    
    // القسم
    public string Category { get; set; }
    public string CategoryValue { get; set; }
    
    // الأسعار
    public decimal Price { get; set; }
    public decimal? PriceAfterDiscount { get; set; }
    
    // SKU
    public string SKU { get; set; }
    
    // التقييم
    public decimal AverageRate { get; set; }
    public decimal TotalRatePeople { get; set; }
    
    // الصور
    public List<ProductImageDto> Images { get; set; }
    
    // الألوان
    public List<ProductColorDto> Colors { get; set; }
    
    // الـ Variants
    public List<DashboardVariantDto> Variants { get; set; }
    
    // معلومات إضافية
    public DateTime Created { get; set; }
    public int TotalStock { get; set; }
    public string Status { get; set; } // In Stock, Low Stock, Out of Stock
}

// DTO لإضافة منتج جديد
public class AddProductDto
{
    // Basic Information
    public string NameAR { get; set; }
    public string NameEN { get; set; }
    public string DescriptionAR { get; set; }
    public string DescriptionEN { get; set; }
    public string CategoryValue { get; set; }
    public decimal Price { get; set; }
    public decimal? PriceAfterDiscount { get; set; }
    public string SKU { get; set; }
    
    // Images - يمكن رفع multiple files
    public List<IFormFile>? ImageFiles { get; set; }
    
    // أو استخدام روابط
    public List<string>? ImageUrls { get; set; }
}

// DTO لتعديل منتج
public class UpdateProductDto
{
    public string NameAR { get; set; }
    public string NameEN { get; set; }
    public string DescriptionAR { get; set; }
    public string DescriptionEN { get; set; }
    public string CategoryValue { get; set; }
    public decimal Price { get; set; }
    public decimal? PriceAfterDiscount { get; set; }
    public string SKU { get; set; }
    
    // لإضافة صور جديدة
    public List<IFormFile>? NewImageFiles { get; set; }
    public List<string>? NewImageUrls { get; set; }
    
    // لحذف صور موجودة (IDs)
    public List<int>? DeleteImageIds { get; set; }
}

// DTO للألوان في Dashboard
public class ProductColorDto
{
    public int Id { get; set; }
    public string ColorAR { get; set; }
    public string ColorEN { get; set; }
}

// DTO لإضافة لون
public class AddProductColorDto
{
    public int ProductId { get; set; }
    public string ColorAR { get; set; }
    public string ColorEN { get; set; }
}

// DTO للـ Variants في Dashboard
public class DashboardVariantDto
{
    public int Id { get; set; }
    public string SizeAR { get; set; }
    public string SizeEN { get; set; }
    public string TypeAR { get; set; }
    public string TypeEN { get; set; }
    public decimal Price { get; set; }
    public decimal? PriceBeforeDiscount { get; set; }
  //  public int? Quantity { get; set; }
}

// DTO لإضافة Variant
public class AddProductVariantDto
{
    public int ProductId { get; set; }
    public string SizeAR { get; set; }
    public string SizeEN { get; set; }
    public string TypeAR { get; set; }
    public string TypeEN { get; set; }
    public decimal Price { get; set; }
    public decimal? PriceBeforeDiscount { get; set; }
    public int Quantity { get; set; }
}
