using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserInformation.Model
{
    public class Hobby
    {
        public int Id { get; set; }
        [Required]
        public string HobbyName { get; set; }


  



    }
}
