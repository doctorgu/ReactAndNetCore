using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcMovieCompare.Data;
using MvcMovieCompare.Models;

namespace MvcMovieCompare.Controllers
{
    public class MoviesController : Controller
    {
        private readonly MvcMovieContext _context;

        public MoviesController(MvcMovieContext context)
        {
            _context = context;
        }

        // GET: Movies
        public async Task<IActionResult> Index()
        {
            /*
            SELECT [m].[Id], [m].[Genre], [m].[Price], [m].[ReleaseDate], [m].[Title]
            FROM [Movie] AS [m]
            */
            return View(await _context.Movie.ToListAsync());
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
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

        // GET: Movies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,ReleaseDate,Genre,Price")] Movie movie)
        {
            /*
            INSERT INTO [Movie] ([Genre], [Price], [ReleaseDate], [Title])
            VALUES (@p0, @p1, @p2, @p3);

            SELECT [Id]
            FROM [Movie]
            WHERE @@ROWCOUNT = 1 AND [Id] = scope_identity();
            */
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
            /*
            SELECT TOP(1) [m].[Id], [m].[Genre], [m].[Price], [m].[ReleaseDate], [m].[Title]
            FROM [Movie] AS [m]
            WHERE [m].[Id] = @__p_0
            */
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
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,ReleaseDate,Genre,Price")] Movie movie)
        {
            /*
            UPDATE [Movie] SET [Genre] = @p0, [Price] = @p1, [ReleaseDate] = @p2, [Title] = @p3
            WHERE [Id] = @p4;
            SELECT @@ROWCOUNT;
            */
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
            /*
            SELECT TOP(1) [m].[Id], [m].[Genre], [m].[Price], [m].[ReleaseDate], [m].[Title]
            FROM [Movie] AS [m]
            WHERE [m].[Id] = @__id_0
            */
            if (id == null)
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
            /*
            SELECT TOP(1) [m].[Id], [m].[Genre], [m].[Price], [m].[ReleaseDate], [m].[Title]
            FROM [Movie] AS [m]
            WHERE [m].[Id] = @__p_0

            DELETE FROM [Movie]
            WHERE [Id] = @p0;
            SELECT @@ROWCOUNT;
            */
            var movie = await _context.Movie.FindAsync(id);
            _context.Movie.Remove(movie);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
            return _context.Movie.Any(e => e.Id == id);
        }
    }
}
