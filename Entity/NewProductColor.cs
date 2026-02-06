using WallsShop.Properties.Entity;

namespace WallsShop.Entity
{
    public class NewProductColor
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int ColorId { get; set; }
        public Color Color { get; set; }
    }

}
