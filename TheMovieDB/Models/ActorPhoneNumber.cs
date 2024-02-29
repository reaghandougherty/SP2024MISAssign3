using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheMovieDB.Models
{
    public class ActorPhoneNumber
    {
        public int Id { get; set; }

        public string? PhoneNumber { get; set; }
        public string? Description { get; set; }


        [ForeignKey("Actor")]
        public int? ActorID { get; set; }
        public Actor? Actor { get; set; }
    }
}
