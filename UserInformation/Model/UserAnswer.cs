using System.ComponentModel.DataAnnotations.Schema;

namespace UserInformation.Model
{
    public class UserAnswer
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int QuestionId { get; set; }
        public string AnswerText { get; set; }
     
    }
}
