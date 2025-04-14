namespace UserInformation.Model
{
    public class Product
    {
        public int Id { get; set; }
        public string? ProductName { get; set; }
        public decimal? UnitPrice { get; set; }
        public bool Discontinued { get; set; }
    }
}
