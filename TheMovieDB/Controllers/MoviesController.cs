using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TheMovieDB.Data;
using TheMovieDB.Models;
using VaderSharp2;

namespace TheMovieDB.Controllers
{
    public class MoviesController : Controller
    {
        public async Task<IActionResult> GetMoviePoster(int id)
        {
            var movie = await _context.Movie.FirstOrDefaultAsync(x => x.Id == id);
            if(movie == null)
            {
                return NotFound();
            }
            var posterData = movie.Poster;
            return File(posterData, "image/jpg");
        }

        private readonly ApplicationDbContext _context;

        public MoviesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Movies
        public async Task<IActionResult> Index()
        {
              return _context.Movie != null ? 
                          View(await _context.Movie.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Movie'  is null.");
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Movie == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            MovieDetailsVM mdVM = new MovieDetailsVM();
            mdVM.movie = movie;

            var actors = new List<Actor>();

            actors = await (from a in _context.Actor join am in _context.ActorMovie on a.Id equals am.ActorID where am.MovieID == id select a).ToListAsync();

            mdVM.actors = actors;

            var queryText = movie.Title;
            var pageContents = await SearchWikipediaAsync(queryText);

            var sentimentList = new List<string>();
            var postList = new List<string>();
            var analyzer = new SentimentIntensityAnalyzer();

            foreach (var pageContent in pageContents)
            {
                // Limit the snippet length to 1500 characters
                var snippet = pageContent.Length > 1500 ? pageContent.Substring(0, 1500) : pageContent;

                // Perform sentiment analysis
                var results = analyzer.PolarityScores(snippet);
                double sentimentScore = results.Compound;

                if (sentimentScore != 0)
                {
                    sentimentList.Add(sentimentScore.ToString() + ", " + CategorizeSentiment(sentimentScore));
                    postList.Add(snippet);
                }
            }

            mdVM.Sentiments = sentimentList;
            mdVM.Posts = postList;

            return View(mdVM);

        }

        //added in code from bb

        public static readonly HttpClient client = new HttpClient();
        public static async Task<List<string>> SearchWikipediaAsync(string queryText)
        {

            string baseUrl = "https://en.wikipedia.org/w/api.php";
            string url = $"{baseUrl}?action=query&list=search&srlimit=100&srsearch={Uri.EscapeDataString(queryText)}&format=json";
            List<string> textToExamine = new List<string>();
            try
            {
                // Ask Wikipedia for a list of pages that relate to the query
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                var jsonDocument = JsonDocument.Parse(responseBody);
                var searchResults = jsonDocument.RootElement.GetProperty("query").GetProperty("search");
                foreach (var item in searchResults.EnumerateArray())
                {
                    var pageId = item.GetProperty("pageid").ToString();
                    // Ask Wikipedia for the text of each page in the query results
                    string pageUrl = $"{baseUrl}?action=query&pageids={pageId}&prop=extracts&explaintext=1&format=json";
                    HttpResponseMessage pageResponse = await client.GetAsync(pageUrl);
                    pageResponse.EnsureSuccessStatusCode();
                    string pageResponseBody = await pageResponse.Content.ReadAsStringAsync();
                    var jsonPageDocument = JsonDocument.Parse(pageResponseBody);
                    var pageContent = jsonPageDocument.RootElement.GetProperty("query").GetProperty("pages").GetProperty(pageId).GetProperty("extract").GetString();
                    textToExamine.Add(pageContent);
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
            return textToExamine;
        }

        public static string CategorizeSentiment(double sentiment)
        {
            if (sentiment >= -1 && sentiment < -0.6)
                return "Extremely Negative";
            else if (sentiment >= -0.6 && sentiment < -0.2)
                return "Very Negative";
            else if (sentiment >= -0.2 && sentiment < 0)
                return "Slightly Negative";
            else if (sentiment >= 0 && sentiment < 0.2)
                return "Slightly Positive";
            else if (sentiment >= 0.2 && sentiment < 0.6)
                return "Very Positive";
            else if (sentiment >= 0.6 && sentiment < 1)
                return "Highly Positive";
            else
                return "Invalid sentiment value";
        }


        //added in code from bb

        /*
        var actors = new List<Actor>();
            ////Option 1
            actors = await (from actor in _context.Actor
                            join am in _context.ActorMovie on actor.Id equals am.ActorID
                            where am.MovieID == id
                            select actor)
                            .ToListAsync();

            ////Option 2
            //actors = await _context.ActorMovie.Where(am => am.MovieID == id)
            //                                .Include(a => a.Actor)
            //                                .Select(a => a.Actor)
            //                                .ToListAsync();

            movieDetailsVM.actors = actors;

            return View(movieDetailsVM);
        }*/

        // GET: Movies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,IMDB,Genre,Year,Description,Poster")] Movie movie, IFormFile Poster)
        {
            ModelState.Remove(nameof(Movie.Poster));

            if (ModelState.IsValid)
            {
                if(Poster != null && Poster.Length > 0)
                {
                    var memoryStream = new MemoryStream();
                    await Poster.CopyToAsync(memoryStream);
                    movie.Poster = memoryStream.ToArray();
                }
                else
                {
                    movie.Poster = new byte[0];
                }

                _context.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Movie == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,IMDB,Genre,Year,Description,Poster")] Movie movie)
        {
            if (id != movie.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Movie == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Movie == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Movie'  is null.");
            }
            var movie = await _context.Movie.FindAsync(id);
            if (movie != null)
            {
                _context.Movie.Remove(movie);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
          return (_context.Movie?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
