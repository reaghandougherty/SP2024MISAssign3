using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TheMovieDB.Data;
using TheMovieDB.Models;

namespace TheMovieDB.Controllers
{
    public class ActorPhoneNumbersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ActorPhoneNumbersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ActorPhoneNumbers
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ActorPhoneNumber.Include(a => a.Actor);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ActorPhoneNumbers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ActorPhoneNumber == null)
            {
                return NotFound();
            }

            var actorPhoneNumber = await _context.ActorPhoneNumber
                .Include(a => a.Actor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (actorPhoneNumber == null)
            {
                return NotFound();
            }

            return View(actorPhoneNumber);
        }

        // GET: ActorPhoneNumbers/Create
        public IActionResult Create()
        {
            ViewData["ActorID"] = new SelectList(_context.Actor, "Id", "Name");
            return View();
        }

        // POST: ActorPhoneNumbers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PhoneNumber,Description,ActorID")] ActorPhoneNumber actorPhoneNumber)
        {
            if (ModelState.IsValid)
            {
                _context.Add(actorPhoneNumber);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ActorID"] = new SelectList(_context.Actor, "Id", "Name", actorPhoneNumber.ActorID);
            return View(actorPhoneNumber);
        }

        // GET: ActorPhoneNumbers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ActorPhoneNumber == null)
            {
                return NotFound();
            }

            var actorPhoneNumber = await _context.ActorPhoneNumber.FindAsync(id);
            if (actorPhoneNumber == null)
            {
                return NotFound();
            }
            ViewData["ActorID"] = new SelectList(_context.Actor, "Id", "Name", actorPhoneNumber.ActorID);
            return View(actorPhoneNumber);
        }

        // POST: ActorPhoneNumbers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PhoneNumber,Description,ActorID")] ActorPhoneNumber actorPhoneNumber)
        {
            if (id != actorPhoneNumber.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(actorPhoneNumber);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ActorPhoneNumberExists(actorPhoneNumber.Id))
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
            ViewData["ActorID"] = new SelectList(_context.Actor, "Id", "Name", actorPhoneNumber.ActorID);
            return View(actorPhoneNumber);
        }

        // GET: ActorPhoneNumbers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ActorPhoneNumber == null)
            {
                return NotFound();
            }

            var actorPhoneNumber = await _context.ActorPhoneNumber
                .Include(a => a.Actor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (actorPhoneNumber == null)
            {
                return NotFound();
            }

            return View(actorPhoneNumber);
        }

        // POST: ActorPhoneNumbers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ActorPhoneNumber == null)
            {
                return Problem("Entity set 'ApplicationDbContext.ActorPhoneNumber'  is null.");
            }
            var actorPhoneNumber = await _context.ActorPhoneNumber.FindAsync(id);
            if (actorPhoneNumber != null)
            {
                _context.ActorPhoneNumber.Remove(actorPhoneNumber);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ActorPhoneNumberExists(int id)
        {
          return (_context.ActorPhoneNumber?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
