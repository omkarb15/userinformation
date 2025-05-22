namespace UserInformation.Model
{
    public class SankeyFlow
    {
        public int Id { get; set; }
        public string? from { get; set; }
        public string? to { get; set; }
        public int value { get; set; }
    }
}
