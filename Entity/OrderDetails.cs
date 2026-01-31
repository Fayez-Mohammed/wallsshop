//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
//using WallsShop.Properties.Entity;

//namespace WallsShop.Entity;

//public class OrderDetails
//{
//    [Key]
//    public int Id { get; set; }

//    [Required]
//    [ForeignKey("Order")]
//    public string OrderId { get; set; }

//    [Required]
//    public int ProductId { get; set; }

//    [MaxLength(1000)]
//    public int Quantity { get; set; }

//    [MaxLength(1000)]
//    public string UserAddress { get; set; }


//    [MaxLength(1000)]
//    public string PhoneNumber { get; set; }

//    [ForeignKey("OrderId")]

//    public virtual Order Order { get; set; }

//    [ForeignKey("ProductId")]
//    public virtual Product Product { get; set; }
//}



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
    public int OrderId { get; set; }
    public string ProductName { get; set; }
    [Required]
    public int ProductId { get; set; }

    [MaxLength(1000)]
    public int Quantity { get; set; }
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }
    public int? VariantId { get; set; }
    public string? Color { get; set; }
    [ForeignKey("OrderId")]

    public virtual Order Order { get; set; }

    [ForeignKey("ProductId")]
    public virtual Product Product { get; set; }
}