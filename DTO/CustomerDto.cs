namespace WallsShop.DTO
{
    internal class CustomerDto
    {
        public int RowNum { get; set; }   
        public string Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Status { get; set; }
    }

    //internal class CustomerDto
    //{
    //    public string Id { get; set; }
    //    public string UserName { get; set; }
    //    public string Email { get; set; }
    //    public string PhoneNumber { get; set; }
    //}
}