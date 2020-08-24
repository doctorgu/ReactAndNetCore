using Microsoft.AspNetCore.Mvc;
using MvcMovieCompare.Data;
using MvcMovieCompare.Models;

namespace MvcMovieCompare.Controllers
{
    public class Movies2Controller : Controller
    {
        private readonly MvcMovie2Context _context;

        public Movies2Controller()
        {
            _context = new MvcMovie2Context();
        }

        // GET: Movies2
        public IActionResult Index()
        {
            return View(_context.ToList());
        }

        // GET: Movies2/Details/5
        public IActionResult Details(int id)
        {
            var movie = _context.FirstOrDefault(id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // GET: Movies2/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Movies2/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,Title,ReleaseDate,Genre,Price")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                _context.Add(movie);
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movies2/Edit/5
        public IActionResult Edit(int id)
        {
            var movie = _context.FirstOrDefault(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        // POST: Movies2/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Title,ReleaseDate,Genre,Price")] Movie movie)
        {
            if (id != movie.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(movie);
                return RedirectToAction(nameof(Index));
            }

            return View(movie);
        }

        // GET: Movies2/Delete/5
        public IActionResult Delete(int id)
        {
            var movie = _context.FirstOrDefault(id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Movies2/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var movie = _context.FirstOrDefault(id);
            _context.Remove(movie);
            return RedirectToAction(nameof(Index));
        }
    }
}
