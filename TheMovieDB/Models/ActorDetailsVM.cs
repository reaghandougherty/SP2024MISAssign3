namespace TheMovieDB.Models
{
    public class ActorDetailsVM
    {
        public Actor actor {  get; set; }
        public List<ActorPhoneNumber> phoneNumbers { get; set; }
    }
}
