using System.ComponentModel.DataAnnotations;
using WallsShop.Entity;

namespace WallsShop.Properties.Entity;

public class Order
{
    [Key] 
    public string Id { get; set; } 
    
    public DateTime OrderDate { get; set; }

    [MaxLength(10)]
    public string Status { get; set; } = "Pending";
    
    
    public virtual List<OrderDetails> OrderDetailsList { get; set; } = new();
}