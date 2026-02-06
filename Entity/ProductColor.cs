using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WallsShop.Properties.Entity;

namespace WallsShop.Entity;

public class ProductColor
{
    [Key]
    public int Id { get; set; }
    
    [ForeignKey("Product")]
    public int ProductId { get; set; }
    
    public string Color { get; set; }
    public string EnglishColor { get; set; } = string.Empty;

    public string LanguageCode { get; set; }
    
    public virtual Product Product { get; set; }
}