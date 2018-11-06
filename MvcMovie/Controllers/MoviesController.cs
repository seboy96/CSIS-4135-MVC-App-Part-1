using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MvcMovie.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MvcMovie.Controllers
{
    public class MoviesController : Controller
    {
        private readonly MvcMovieContext _context;
        private string apiKey;

        public MoviesController(MvcMovieContext context, IConfiguration config)
        {
            _context = context;
            apiKey = config.GetValue<string>("APIKey");
        }

        // Requires using Microsoft.AspNetCore.Mvc.Rendering;
        [Route("MovieHome")]
        public async Task<IActionResult> Index(string movieGenre, string searchString)
        {
            // Use LINQ to get list of genres.
            IQueryable<string> genreQuery = from m in _context.Movie
                                            orderby m.Genre
                                            select m.Genre;

            var movies = from m in _context.Movie
                         select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                movies = movies.Where(s => s.Title.Contains(searchString));
            }

            if (!String.IsNullOrEmpty(movieGenre))
            {
                movies = movies.Where(x => x.Genre == movieGenre);
            }

            var movieGenreVM = new MovieGenreViewModel();
            movieGenreVM.genres = new SelectList(await genreQuery.Distinct().ToListAsync());
            movieGenreVM.movies = await movies.ToListAsync();
            return View(movieGenreVM);
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.ID == id);
            if (movie == null)
            {
                return NotFound();
            }

            var reviewVM = new ReviewViewModel();
            reviewVM.movie = movie;
            var reviews = from r in _context.Review
                          select r;
            reviews = reviews.Where(r => r.MovieID == id);
            reviewVM.reviews = await reviews.ToListAsync();
            return View(reviewVM);
        }

        // GET: Movies/Create
        public IActionResult Create()
        {
            List<Rating> ratings = new List<Rating>();
            Rating ratingItem;
            var strArr = new string[] { "G", "PG", "PG-13", "R", "NC-17", "NR" };
            for (int index = 0; index < strArr.Length; index++)
            {
                ratingItem = new Rating();
                ratingItem.Text = strArr[index];
                ratingItem.Value = (index + 1).ToString();
                ratings.Add(ratingItem);
            }
            ViewData["Ratings"] = ratings;
            
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Title,ReleaseDate,Genre,Price,Rating,Poster")] Movie movie)
        {
            List<Rating> ratings = new List<Rating>();
            Rating ratingItem;
            var strArr = new string[] { "G", "PG", "PG-13", "R", "NC-17", "NR" };
            for (int index = 0; index < strArr.Length; index++)
            {
                ratingItem = new Rating();
                ratingItem.Text = strArr[index];
                ratingItem.Value = (index + 1).ToString();
                ratings.Add(ratingItem);
            }
            ViewData["Ratings"] = ratings;

            var movieResult = await _context.Movie
                .FirstOrDefaultAsync(m => m.Title == movie.Title);
            if(!(movieResult == null))
            {
                var quote = '"';
                ModelState.AddModelError("Title", quote + movie.Title + quote + " is already in database.");
                return View("Create");
            }
            if (ModelState.IsValid)
            {
                _context.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            List<Rating> ratings = new List<Rating>();
            Rating ratingItem;
            var strArr = new string[] { "G", "PG", "PG-13", "R", "NC-17", "NR" };
            for (int index = 0; index < strArr.Length; index++)
            {
                ratingItem = new Rating();
                ratingItem.Text = strArr[index];
                ratingItem.Value = (index + 1).ToString();
                ratings.Add(ratingItem);
            }
            ViewData["Ratings"] = ratings;
            if (id == null)
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
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Title,ReleaseDate,Genre,Price,Rating")] Movie movie)
        {
            List<Rating> ratings = new List<Rating>();
            Rating ratingItem;
            var strArr = new string[] { "G", "PG", "PG-13", "R", "NC-17", "NR" };
            for (int index = 0; index < strArr.Length; index++)
            {
                ratingItem = new Rating();
                ratingItem.Text = strArr[index];
                ratingItem.Value = (index + 1).ToString();
                ratings.Add(ratingItem);
            }
            ViewData["Ratings"] = ratings;
            if (id != movie.ID)
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
                    if (!MovieExists(movie.ID))
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
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.ID == id);
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
            var movie = await _context.Movie.FindAsync(id);
            _context.Movie.Remove(movie);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
            return _context.Movie.Any(e => e.ID == id);
        }

        public async Task<IActionResult> OMDB(String movieTitle)
        {
            List<Rating> ratings = new List<Rating>();
            Rating ratingItem;
            var strArr = new string[] { "G", "PG", "PG-13", "R", "NC-17", "NR" };
            for (int index = 0; index < strArr.Length; index++)
            {
                ratingItem = new Rating();
                ratingItem.Text = strArr[index];
                ratingItem.Value = (index + 1).ToString();
                ratings.Add(ratingItem);
            }
            ViewData["Ratings"] = ratings;
            HttpClient client = new HttpClient();
            string url = "http://www.omdbapi.com/?t=" + movieTitle + "&apikey=" + apiKey;
            var response = await client.GetAsync(url);
            var data = await response.Content.ReadAsStringAsync();
            var json = JsonConvert.DeserializeObject(data).ToString();
            dynamic omdbMovie = JObject.Parse(json);

            Movie movie = new Movie
            {
                Title = omdbMovie["Title"],
                Genre = omdbMovie["Genre"],
                ReleaseDate = omdbMovie["Released"],
                Rating = omdbMovie["Rated"],
                Price = 0,
                Poster = omdbMovie["Poster"]
            };
            if (omdbMovie["Response"] == "False")
            {
                var quote = '"';
                ModelState.AddModelError("Title", quote + movieTitle + quote + " was not found.");
                return View("Create");
            }
            else
            {
                if (movie.Poster == null)
                {
                    movie.Poster = "N/A";
                }
                var rating = false;
                if(!(movie.Genre == null))
                {
                    var temp = movie.Genre.Split(',');
                    movie.Genre = temp[0];
                }
                for (int index = 0; index < strArr.Length; index++)
                {
                    if(movie.Rating == strArr[index])
                    {
                        rating = true;
                    }
                }
                if(!rating)
                {
                    movie.Rating = "NR";
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(movie);
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!MovieExists(movie.ID))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                }

                ModelState.AddModelError("Price", "PRICE REQUIRED");
                return View("Create", movie);
            }
        }
    }
}


