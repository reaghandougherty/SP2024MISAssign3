namespace TheMovieDB.Models
{
    public class ActorDetailsVM
    {
        public Actor actor {  get; set; }
        public List<ActorPhoneNumber> phoneNumbers { get; set; }
        public List<string> Sentiments { get; set; }
        public List<string> Posts { get; set; }

        public List<string> actors { get; set; }
    }
}
