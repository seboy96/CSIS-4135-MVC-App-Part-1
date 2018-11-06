using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcMovie.Models;

namespace MvcMovie.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly MvcMovieContext _context;

        public ReviewsController(MvcMovieContext context)
        {
            _context = context;
        }

        // GET: Reviews
        public async Task<IActionResult> Index(string sortOrder)
        {
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.MovieSortParm = sortOrder == "Movie" ? "movie_desc" : "Movie";
            var reviews = from r in _context.Review
                          select r;
            ViewData["Glyph"] = "";
            ViewData["indicator"] = 0;
            switch (sortOrder)
            {
                case "name_desc":
                    reviews = reviews.OrderByDescending(r => r.Name).Include(r => r.Movie); ;
                    ViewData["Glyph"] = "glyphicon glyphicon-triangle-bottom";
                    ViewData["indicator"] = 1;
                    break;
                case "Movie":
                    reviews = reviews.OrderBy(r => r.Movie.Title).Include(r => r.Movie); ;
                    ViewData["Glyph"] = "glyphicon glyphicon-triangle-top";
                    ViewData["indicator"] = 2;
                    break;
                case "movie_desc":
                    reviews = reviews.OrderByDescending(r => r.Movie.Title).Include(r => r.Movie); ;
                    ViewData["Glyph"] = "glyphicon glyphicon-triangle-bottom";
                    ViewData["indicator"] = 2;
                    break;
                default:
                    reviews = reviews.OrderBy(r => r.Name).Include(r => r.Movie);
                    ViewData["Glyph"] = "glyphicon glyphicon-triangle-top";
                    ViewData["indicator"] = 1;
                    break;
            }
            //var mvcMovieContext = _context.Review.Include(r => r.Movie);
            //return View(await mvcMovieContext.ToListAsync());
            return View(reviews.ToList());
        }

        // GET: Reviews/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Review
                .Include(r => r.Movie)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        // GET: Reviews/Create
        public IActionResult Create(int id)
        {
            ViewData["MovieID"] = id;
            return View();
        }

        // POST: Reviews/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Comment,MovieID")] Review review)
        {
            if (ModelState.IsValid)
            {
                _context.Add(review);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), "Movies", new { id = review.MovieID });
            }
            ViewData["MovieID"] = new SelectList(_context.Movie, "ID", "Genre", review.MovieID);
            return View(review);
        }

        // GET: Reviews/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Review.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }
            ViewData["MovieID"] = new SelectList(_context.Movie, "ID", "Genre", review.MovieID);
            ViewData["MoviesID"] = review.MovieID;
            return View(review);
        }

        // POST: Reviews/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Comment,MovieID")] Review review)
        {
            if (id != review.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(review);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReviewExists(review.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Details), "Movies", new { id = review.MovieID });
            }
            ViewData["MovieID"] = review.MovieID;
            return View(review);
        }

        // GET: Reviews/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Review
                .Include(r => r.Movie)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (review == null)
            {
                return NotFound();
            }
            ViewData["MoviesID"] = review.MovieID;
            return View(review);
        }

        // POST: Reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var review = await _context.Review.FindAsync(id);
            _context.Review.Remove(review);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), "Movies", new { id = review.MovieID });
        }

        private bool ReviewExists(int id)
        {
            return _context.Review.Any(e => e.ID == id);
        }
    }
}
