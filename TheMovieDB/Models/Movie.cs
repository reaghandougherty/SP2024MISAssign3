using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace TheMovieDB.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string IMDB { get; set; }
        public string Genre { get; set; }
        public int Year { get; set; }
        [DataType(DataType.Upload)]
        [DisplayName("Poster")]
        public byte[]? Poster { get; set; }
        public string Description { get; set; }
        public string Director { get; set; }


    }
}