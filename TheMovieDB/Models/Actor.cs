using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace TheMovieDB.Models
{
    public class Actor
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        [DisplayName("Gender")]
        public string? Gender { get; set; }
        [DisplayName("Age")]
        public int Age { get; set; }
        [DisplayName("IMDB")]
        public string? IMDB { get; set; }


        [DataType(DataType.Upload)]
        [DisplayName("Actor Image")]
        public byte[]? ActorImage { get; set; }

    }
}