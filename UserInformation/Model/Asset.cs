using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserInformation.Model
{
    public class Asset
    {

        public int? Id { get; set; }

        public string? FirstName { get; set; }

        public string? SurName { get; set; }

        public DateTime DOB { get; set; }

        public string? Gender { get; set; }

        public string? EmialId { get; set; }

        public string? UserName { get; set; }

        public string? PassWord { get; set; }

        public string? ProfilImage { get; set; }
        [NotMapped]
        public string? HobbyId { get; set; }

        [NotMapped]
        public string? HobbyName { get; set; }




    }
}
