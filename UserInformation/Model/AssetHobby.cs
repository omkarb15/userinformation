using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UserInformation.Model
{
    public class AssetHobby
    {

        [Key]
        public int Id { get; set; }

    
        public int? AssetId { get; set; }
      

        public int? HobbyId { get; set; }
    
        

    }
}
