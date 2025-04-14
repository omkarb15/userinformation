namespace UserInformation.Model
{
    public class CheckBoxTree
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int? ParentId { get; set; }
        public  bool IsChecked { get; set; }
    }
}
