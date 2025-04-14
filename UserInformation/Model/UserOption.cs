using System.ComponentModel.DataAnnotations.Schema;

namespace UserInformation.Model
{
    public class UserOption
    {
        public int Id { get; set; }
        [ForeignKey("Asset")]
        public int UserId { get; set; }
        [ForeignKey("QuestionOpt")]
        public int QuestionId { get; set; }
        [ForeignKey("Option")]
        public int OptionId { get; set; }
    }
}
