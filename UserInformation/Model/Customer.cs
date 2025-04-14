namespace UserInformation.Model
{
    public class Customer
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string ContactTitle { get; set; }
        public string City { get; set; }

        public int DisplayOrder { get; set; }

    }
}
