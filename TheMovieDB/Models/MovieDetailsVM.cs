namespace TheMovieDB.Models
{
    public class MovieDetailsVM
    {
        public Movie movie { get; set; }

        public string? Sentiment { get; set; }
        public List<Actor> actors { get; set;}
    }
}
