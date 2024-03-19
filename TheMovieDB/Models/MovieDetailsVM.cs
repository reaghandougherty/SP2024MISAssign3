namespace TheMovieDB.Models
{
    public class MovieDetailsVM
    {
        public Movie movie { get; set; }
        public List<string> Sentiments { get; set; }
        public List<string> Posts { get; set; }
        public string? Sentiment { get; set; }
        public List<Actor> actors { get; set;}
    }
}
