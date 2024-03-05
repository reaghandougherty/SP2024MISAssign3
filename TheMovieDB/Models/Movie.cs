namespace TheMovieDB.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string IMDB { get; set; }
        public string Genre { get; set; }
        public string Year { get; set; }
        public string Poster { get; set; }
        public string Description { get; set; }
        public string Director { get; set; }


    }
}