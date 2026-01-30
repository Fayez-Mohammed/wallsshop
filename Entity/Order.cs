//using System.ComponentModel.DataAnnotations;
//using WallsShop.Entity;

//namespace WallsShop.Properties.Entity;

//public class Order
//{
//    [Key] 
//    public string Id { get; set; } 

//    public DateTime OrderDate { get; set; }

//    [MaxLength(10)]
//    public string Status { get; set; } = "Pending";


//    public virtual List<OrderDetails> OrderDetailsList { get; set; } = new();
//}



using System.ComponentModel.DataAnnotations;
using WallsShop.Entity;

namespace WallsShop.Properties.Entity;

public class Order
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? UserId { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    [MaxLength(10)]
    public string Status { get; set; } = "Pending";


    [MaxLength(1000)]

    public string? ShippingAddress { get; set; }
    public string? ReceiverPhone { get; set; }
    public string? ReceiverName { get; set; }
    [MaxLength(1000)]
    public decimal TotalAmount { get; set; }
    public virtual List<OrderDetails> OrderDetailsList { get; set; } = new();
}