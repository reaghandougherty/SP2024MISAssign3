using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TheMovieDB.Models;

namespace TheMovieDB.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Movie>? Movie { get; set; }
        public DbSet<Actor>? Actor { get; set; }
        public DbSet<ActorMovie>? ActorMovie { get; set; }
        public DbSet<ActorPhoneNumber>? ActorPhoneNumber { get; set; }
    }
}