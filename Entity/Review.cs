using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WallsShop.Properties.Entity;

namespace WallsShop.Entity;

public class Review
{
    public int Id { get; set; }
    
    
    [MaxLength(200)]
    public required string Comment { get; set; } 
    
     
    public string UserName { get; set; }
    
    [Required]
    
    public int ProductId { get; set; }
    
    
    public DateTime ReviewDate { get; set; }
    
    
    public decimal Rate { get; set; }
}