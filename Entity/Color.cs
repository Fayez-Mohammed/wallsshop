namespace WallsShop.Entity
{
    public class Color
    {
        public int Id { get; set; }

        public string ColorName { get; set; }
      //  public string LanguageCode { get; set; }
        public string? EnglishColor { get; set; }

        // navigation
        public ICollection<NewProductColor> NewProductColors { get; set; } = new List<NewProductColor>();
    }

}
