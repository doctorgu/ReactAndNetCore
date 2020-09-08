using Microsoft.AspNetCore.Mvc;
using MovieApi.Data;
using MovieApi.Models;
using System.Collections;
using System.Collections.Generic;

namespace MovieApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MoviesController : Controller
    {
        private readonly MovieContext _context;

        public MoviesController()
        {
            _context = new MovieContext();
        }

        // GET: Movies
        [HttpGet("/movies/")]
        public IEnumerable<Movie> Index()
        {
            return _context.ToList();
        }

        // GET: Movies/Details/5
        [HttpGet("/movies/details/{id}")]
        public Movie Details(int id)
        {
            var movie = _context.FirstOrDefault(id);
            if (movie == null)
            {
                return null;
            }

            return movie;
        }

        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("/movies/create/")]
        //[ValidateAntiForgeryToken]
        public bool Create([Bind("Id,Title,ReleaseDate,Genre,Price")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                _context.Add(movie);
                return true;
            }
            return false;
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("/movies/edit/")]
        //[ValidateAntiForgeryToken]
        public bool Edit([Bind("Id,Title,ReleaseDate,Genre,Price")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                _context.Update(movie);
                return true;
            }

            return false;
        }

        // POST: Movies/Delete/5
        [HttpPost("/movies/delete/{id}")]
        //[ValidateAntiForgeryToken]
        public bool DeleteConfirmed(int id)
        {
            var movie = _context.FirstOrDefault(id);
            _context.Remove(movie);
            return true;
        }

        [HttpGet("/routes")]
        public IEnumerable<object> Routes()
        {
            //var Routes = new Route[]
            //{
            //    new Route()
            //    {
            //        Path = "/",
            //        Component = "../layouts/BasicLayout",
            //        Routes = new Route[]
            //        {
            //            new Route()
            //            {
            //                Path = "/dashboard",
            //                Name = "dashboard",
            //                Icon = "dashboard",
            //                Routes = new Route[]
            //                {
            //                    new Route()
            //                    {
            //                        Name = "analysis",
            //                        Path = "/dashboard/analysis",
            //                        Component = "./dashboard/analysis",
            //                    },
            //                    new Route()
            //                    {
            //                        Name = "monitor",
            //                        Path = "/dashboard/monitor",
            //                        Component = "./dashboard/monitor",
            //                    },
            //                    new Route()
            //                    {
            //                        Name = "workplace",
            //                        Path = "/dashboard/workplace",
            //                        Component = "./dashboard/workplace",
            //                    },
            //                },
            //            },
            //        },
            //    },
            //};

            return new[]
            {
                new
                {
                    Path = "/",
                    Component = "../layouts/BasicLayout",
                    Routes = new []
                    {
                        new
                        {
                            Path = "/dashboard",
                            Name = "dashboard",
                            Icon = "dashboard",
                            Routes = new []
                            {
                                new
                                {
                                    Name = "analysis",
                                    Path = "/dashboard/analysis",
                                    Component = "./dashboard/analysis",
                                },
                                new
                                {
                                    Name = "monitor",
                                    Path = "/dashboard/monitor",
                                    Component = "./dashboard/monitor",
                                },
                                new
                                {
                                    Name = "workplace",
                                    Path = "/dashboard/workplace",
                                    Component = "./dashboard/workplace",
                                },
                            },
                        },
                    },
                },
            };

            //return Routes;
        }

        [HttpGet("/authorities")]
        public IEnumerable<object> Authorities()
        {
            return new[]
            {
                new {
                    Path = "/dashboard/analysis",
                    Ids = new [] { "user", "admin", },
                },
                new {
                    Path = "/dashboard/monitor",
                    Ids = new [] { "user", "admin", },
                },
                new {
                    Path = "/dashboard/workplace",
                    Ids = new [] { "admin", },
                },
                new {
                    Path = "/form/basic-form",
                    Ids = new [] { "admin", },
                },
            };
        }
    }
}
