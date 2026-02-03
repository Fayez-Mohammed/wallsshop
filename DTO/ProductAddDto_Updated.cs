namespace WallsShop.DTO;

public class ProductUpdateDto
{
    public string NameAR { get; set; }
    public string NameEN { get; set; }
    public string DescriptionAR { get; set; }
    public string fullDescriptionAR { get; set; } = string.Empty;
    public string DescriptionEN { get; set; }
    public string SKU { get; set; }
    public decimal Price { get; set; }
    public decimal? PriceBeforeDiscount { get; set; }
    public string CategoryValue { get; set; }

    public List<string>? SizesAR { get; set; }
    public List<string>? SizesEN { get; set; }
    public List<string>? TypesAR { get; set; }
    public List<string>? TypesEN { get; set; }
    public List<string>? ColorsAR { get; set; }
    public List<string>? ColorsEN { get; set; }

    // الصور الجديدة
    public List<IFormFile>? NewImageFiles { get; set; }
    public List<string>? NewImageUrls { get; set; }

    // IDs الصور المراد حذفها
    public List<int>? DeleteImageIds { get; set; }
}
public class ProductAddDtoaren
{
    // الأسماء
    public string NameAR { get; set; }
    public string NameEN { get; set; }

    // الأوصاف
    public string DescriptionAR { get; set; }
    public string DescriptionEN { get; set; }

    // معلومات أساسية
    public string SKU { get; set; }
    public decimal Price { get; set; }
    public decimal? PriceBeforeDiscount { get; set; }
    public string CategoryValue { get; set; }

    // الأحجام (JSON string أو array بسيط)
    public List<string> SizesAR { get; set; }
    public List<string> SizesEN { get; set; }

    // الأنواع
    public List<string> TypesAR { get; set; }
    public List<string> TypesEN { get; set; }

    // الألوان
    public List<string> ColorsAR { get; set; }
    public List<string> ColorsEN { get; set; }

    // الصور
    public List<IFormFile>? ImageFiles { get; set; }
    public List<string>? ImageUrls { get; set; }
}
//namespace WallsShop.DTO;


//public class ProductAddDto11
//{
//    // الأسماء
//    public string NameAR { get; set; }
//    public string NameEN { get; set; }

//    // الأوصاف
//    public string DescriptionAR { get; set; }
//    public string DescriptionEN { get; set; }

//    // معلومات أساسية
//    public string SKU { get; set; }
//    public decimal Price { get; set; }
//    public decimal? PriceBeforeDiscount { get; set; }
//    public string CategoryValue { get; set; }

//    // الأحجام (JSON string أو array بسيط)
//    public List<string> SizesAR { get; set; }
//    public List<string> SizesEN { get; set; }

//    // الأنواع
//    public List<string> TypesAR { get; set; }
//    public List<string> TypesEN { get; set; }

//    // الألوان
//    public List<string> ColorsAR { get; set; }
//    public List<string> ColorsEN { get; set; }

//    // الصور
//    public List<IFormFile>? ImageFiles { get; set; }
//    public List<string>? ImageUrls { get; set; }
//}
//public class ProductAddDto22
//{
//    public string NameAR { get; set; }
//    public string NameEN { get; set; }

//    public string FullDescriptionAR { get; set; }
//    public string DescriptionAR { get; set; }
//    public string DescriptionEN { get; set; }

//    public string SKU { get; set; }
//    public decimal Price { get; set; }
//    public decimal? PriceBeforeDiscount { get; set; }
//    public string CategoryValue { get; set; }

//    public List<string> Sizes { get; set; }

//    public List<string> Types { get; set; }

//    public List<string> Colors { get; set; }

//    public List<IFormFile>? ImageFiles { get; set; }
//    public List<string>? ImageUrls { get; set; }
//}

//public class ProductSizeDto2
//{
//    public string SizeAR { get; set; }
//    public string SizeEN { get; set; }
//}

//public class ProductTypeDto2
//{
//    public string TypeAR { get; set; }
//    public string TypeEN { get; set; }
//}

//public class ProductColorDto2
//{
//    public string ColorAR { get; set; }
//    public string ColorEN { get; set; }
//}
