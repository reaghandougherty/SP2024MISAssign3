using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace TheMovieDB.Models
{
    public class Actor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }

        [DataType(DataType.Upload)]
        [DisplayName("Actor Image")]
        public byte[]? ActorImage { get; set; }

        public List<Movie> Movies { get; set; }
    }
}