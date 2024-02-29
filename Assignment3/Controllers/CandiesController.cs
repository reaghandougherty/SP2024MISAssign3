using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Assignment3.Data;
using Assignment3.Models;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Assignment3.Views
{
    public class CandiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CandiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> GetProductPhoto(int id)
        {
            var candy = await _context.Candy
                .FirstOrDefaultAsync(m => m.Id == id);
            if (candy == null)
            {
                return NotFound();
            }
            var imageData = candy.ProductImage;

            return File(imageData, "image/jpg");
        }

        // GET: Candies
        public async Task<IActionResult> Index()
        {
            return View(await _context.Candy.ToListAsync());
        }

        // GET: Candies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var candy = await _context.Candy
                .FirstOrDefaultAsync(m => m.Id == id);
            if (candy == null)
            {
                return NotFound();
            }

            return View(candy);
        }

        // GET: Candies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Candies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Weight,Cost,IsHealthy")] Candy candy, IFormFile ProductImage)
        {
            if (ModelState.IsValid)
            {
                if (ProductImage != null && ProductImage.Length > 0)
                {
                    var memoryStream = new MemoryStream();
                    await ProductImage.CopyToAsync(memoryStream);
                    candy.ProductImage = memoryStream.ToArray();
                }
                _context.Add(candy);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(candy);
        }

        // GET: Candies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var candy = await _context.Candy.FindAsync(id);
            if (candy == null)
            {
                return NotFound();
            }
            return View(candy);
        }

        // POST: Candies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Weight,Cost,IsHealthy")] Candy candy, IFormFile ProductImage)
        {
            if (id != candy.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (ProductImage != null && ProductImage.Length > 0)
                    {
                        var memoryStream = new MemoryStream();
                        await ProductImage.CopyToAsync(memoryStream);
                        candy.ProductImage = memoryStream.ToArray();
                    }
                    else //grab EXISTING photo from DB in case user didn't select a new one
                    {
                        Candy existingProduct = _context.Candy.AsNoTracking().FirstOrDefault(m => m.Id == id);
                        if (existingProduct != null)
                        {
                            candy.ProductImage = existingProduct.ProductImage;
                        }
                    }
                    _context.Update(candy);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CandyExists(candy.Id))
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
            return View(candy);
        }

        // GET: Candies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var candy = await _context.Candy
                .FirstOrDefaultAsync(m => m.Id == id);
            if (candy == null)
            {
                return NotFound();
            }

            return View(candy);
        }

        // POST: Candies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var candy = await _context.Candy.FindAsync(id);
            _context.Candy.Remove(candy);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CandyExists(int id)
        {
            return _context.Candy.Any(e => e.Id == id);
        }
    }
}
