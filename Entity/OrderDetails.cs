using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WallsShop.Properties.Entity;

namespace WallsShop.Entity;

public class OrderDetails
{
    [Key]
    public int Id { get; set; }

    [Required]
    [ForeignKey("Order")]
    public string OrderId { get; set; }
    
    [Required]
    public int ProductId { get; set; }
    
    [MaxLength(1000)]
    public int Quantity { get; set; }
    
    [MaxLength(1000)]
    public string UserAddress { get; set; }
    
    
    [MaxLength(1000)]
    public string PhoneNumber { get; set; }
    
    [ForeignKey("OrderId")]
    
    public virtual Order Order { get; set; }
    
    [ForeignKey("ProductId")]
    public virtual Product Product { get; set; }
}