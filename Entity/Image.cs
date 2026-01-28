using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WallsShop.Properties.Entity;

namespace WallsShop.Entity;

public class Image
{
    public  int Id { get; set; }
    
    public string RelativePath { get; set; }
    
    [Required]
    public int ProductId { get; set; }

      
}