using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using TheMovieDB.Data;
using TheMovieDB.Models;
using VaderSharp2;

namespace TheMovieDB.Controllers
{
    public class ActorsController : Controller
    {

        public async Task<IActionResult> GetActorPhoto(int id)
        {
            var actor = await _context.Actor
                .FirstOrDefaultAsync(m => m.Id == id);
            if (actor == null)
            {
                return NotFound();
            }
            var imageData = actor.ActorImage;

            return File(imageData, "image/jpg");
        }

        private readonly ApplicationDbContext _context;
        public ActorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Actors
        public async Task<IActionResult> Index()
        {
              return _context.Actor != null ? 
                          View(await _context.Actor.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Actor'  is null.");
        }

        // GET: Actors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Actor == null)
            {
                return NotFound();
            }

            var actor = await _context.Actor
                .FirstOrDefaultAsync(m => m.Id == id);
            if (actor == null)
            {
                return NotFound();
            }
            ActorDetailsVM ad = new ActorDetailsVM();   
            ad.actor = actor;

            /*
            var actors = new List<Actor>();

            actors = await (from a in _context.Actor join am in _context.ActorMovie on a.Id equals am.ActorID where am.MovieID == id select a).ToListAsync();

            ad.actors = actors; */

            var queryText = actor.Name;
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

            ad.Sentiments = sentimentList;
            ad.Posts = postList;

            return View(ad);

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

        // GET: Actors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Actors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Gender,Age,IMDB,ActorImage")] Actor actor, IFormFile ActorImage)
        {
            ModelState.Remove(nameof(actor.ActorImage));

            if (ModelState.IsValid)
            {
                if (ActorImage != null && ActorImage.Length > 0)
                {
                    var memoryStream = new MemoryStream();
                    await ActorImage.CopyToAsync(memoryStream);
                    actor.ActorImage = memoryStream.ToArray();
                }
                else
                {
                    actor.ActorImage = new byte[0];
                }
                _context.Add(actor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(actor);
        }

        //[Authorize(Roles = Constants.AdministratorsRole + "," + Constants.ManagersRole)]
        //[Authorize(Roles = "AdministratorRole,ManagerRole")] //this is an OR condition!!!
        //[Authorize(Roles = Constants.AdministratorsRole)]
        //[Authorize(Policy = Constants.ManagerAndAdministrator)]
        //[Authorize(Roles = Constants.Administrator + "," + Constants.Manager)]
        // GET: Actors/Edit/5
        [Authorize(Roles = Constants.AdministratorsRole + "," + Constants.ManagersRole)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Actor == null)
            {
                return NotFound();
            }

            var actor = await _context.Actor.FindAsync(id);
            if (actor == null)
            {
                return NotFound();
            }
            return View(actor);
        }

        // POST: Actors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = Constants.AdministratorsRole + "," + Constants.ManagersRole)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Gender,Age,IMDB,ActorImage")] Actor actor, IFormFile ActorImage)
        {
            if (id != actor.Id)
            {
                return NotFound();
            }

            ModelState.Remove(nameof(actor.ActorImage));

            Actor existingActor = _context.Actor.AsNoTracking().FirstOrDefault(m => m.Id == id);

            if (ActorImage != null && ActorImage.Length > 0)
            {
                var memoryStream = new MemoryStream();
                await ActorImage.CopyToAsync(memoryStream);
                actor.ActorImage = memoryStream.ToArray();
            }
            //grab EXISTING photo from DB in case user didn't upload a new one. Otherwise, the actor will have the photo overwritten with empty
            else if (existingActor != null)
            {
                actor.ActorImage = existingActor.ActorImage;
            }
            else
            {
                actor.ActorImage = new byte[0];
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(actor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ActorExists(actor.Id))
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
            return View(actor);
        }

        // GET: Actors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Actor == null)
            {
                return NotFound();
            }

            var actor = await _context.Actor
                .FirstOrDefaultAsync(m => m.Id == id);
            if (actor == null)
            {
                return NotFound();
            }

            return View(actor);
        }

        // POST: Actors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Actor == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Actor'  is null.");
            }
            var actor = await _context.Actor.FindAsync(id);
            if (actor != null)
            {
                _context.Actor.Remove(actor);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ActorExists(int id)
        {
          return (_context.Actor?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
