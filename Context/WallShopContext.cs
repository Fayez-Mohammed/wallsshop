using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WallsShop.Entity;
using WallsShop.Properties.Entity;

namespace WallsShop.Context;

public class WallShopContext : IdentityDbContext<User>
{
    public WallShopContext(DbContextOptions<WallShopContext> options)
        : base(options)
    {
    }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    
    public DbSet<OrderDetails> OrderDetails { get; set; }
    public DbSet<Review> Reviews { get; set; }
    
    public DbSet<Wishlist> Wishlists { get; set; }
 
    public DbSet<Image> Images { get; set; }
    
    public DbSet<Form> Forms { get; set; }
    public DbSet<ProductColor> Colors { get; set; }
    
    public DbSet<ProductDescription> Descriptions { get; set; }
    
    public DbSet<Variant> Variants { get; set; }
    
    public DbSet<Offer> Offers { get; set; }
    
    public DbSet<Coupon> Coupons { get; set; }
    public DbSet<ProductTranslation> ProductTranslations { get; set; }
    
    public DbSet<CategoryImage> CategoryImages { get; set; }
protected override void OnModelCreating(ModelBuilder mb)
{
    base.OnModelCreating(mb);
    mb.Entity<Wishlist>()
        .Property(w => w.Id)
        .ValueGeneratedOnAdd();

    mb.Entity<Image>()
        .HasOne<Product>() 
        .WithMany(p => p.Images)
        .IsRequired()
        .HasForeignKey("ProductId"); 
    mb.Entity<Review>(entity =>
    {
        // Tell EF to ignore any "UserId" shadow property it's trying to create
        entity.Ignore("UserId"); 

        // Explicitly map the relationship to Product
        entity.HasOne<Product>()
            .WithMany(p => p.Reviews)
            .HasForeignKey(r => r.ProductId);
    });
    
    mb.Entity<Variant>()
        .HasOne(v => v.Product)
        .WithMany(p => p.Variants)
        .HasForeignKey(v => v.ProductId)
        .OnDelete(DeleteBehavior.Cascade);

    mb.Entity<Wishlist>()
        .HasIndex(w => w.UserId)
        .IsUnique(false);
 
     
    mb.Entity<ProductTranslation>()
        .HasOne(t => t.Product)
        .WithMany(p => p.Translations)
        .HasForeignKey(t => t.ProductId)
        .OnDelete(DeleteBehavior.Cascade);

    

    mb.Entity<Product>(entity =>
    {
        entity.Property(e => e.Price).HasPrecision(18, 2);  
        entity.Property(e => e.PriceAfterDiscount).HasPrecision(18, 2);  
        entity.Property(e => e.AverageRate).HasPrecision(3, 2); 
        entity.Property(e => e.RatingSum).HasPrecision(10, 2);
        entity.Property(e => e.TotalRatePeople).HasPrecision(10, 0);
    });

    mb.Entity<Variant>(entity =>
    {
        entity.Property(e => e.Price).HasPrecision(18, 2);
        entity.Property(e => e.DiscountRate).HasPrecision(5, 2);
        entity.Property(e => e.PriceBeforeDiscount).HasPrecision(18, 2);
    });

    mb.Entity<ProductPrice>(entity =>
    {
        entity.Property(e => e.Price).HasPrecision(18, 2);
        entity.Property(e => e.DiscountAmount).HasPrecision(18, 2);
        entity.Property(e => e.PriceBeforeDiscount).HasPrecision(18, 2);
    });
}
}