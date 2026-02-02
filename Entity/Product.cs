using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WallsShop.Entity;

namespace WallsShop.Properties.Entity;

public class Product
{
     [Key]
     public int Id { get; set; }
     
     [MaxLength(100)]
     public string? Name { get; set; }
     public string? NameEN { get; set; }
   
     
     [MaxLength(500)]
     public string SKU { get; set; }
     
     public required decimal Price { get; set; }
     
     public decimal PriceAfterDiscount { get; set; }
     
      
     
  //   [MaxLength(200)]
     
    // public   string Category { get; set; }

     public string FullDescription { get; set; }
     
     public string Descriptions { get; set; }
     public string? DescriptionsEN { get; set; }
     
     [DefaultValue(value: 0.0)]
     public decimal TotalRatePeople { get; set; }
     
     [DefaultValue(value: 0.0)]
     public decimal RatingSum { get; set; }
     
     [DefaultValue(value: 0.0)]
     public decimal AverageRate { get; set; }  
     public DateTime Created { get; set; }
     
   //  public string CategoryValue { get; set; }
    public int? CategoryFK { get; set; }

    [ForeignKey("CategoryFK")]
    public virtual CategoryImage CategoryImage { get; set; }
    public virtual List<ProductPrice>? Prices { get; set; }
     
     public virtual List<ProductType>? Types { get; set; }
     
     public string LanguageCode { get; set; }
     public virtual  List<Image> Images { get; set; }  
     
     public virtual List<ProductColor>? Colors { get; set; }
     
     public virtual List<Variant>? Variants { get; set; }
     
     public virtual List<ProductTranslation>? Translations { get; set; }
     
     public virtual List<Review>? Reviews { get; set; }
      
}



 