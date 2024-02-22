using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using imdb.Data;
using imdb.Models;
using Microsoft.AspNetCore.Authorization;

namespace imdb.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class MovieController : ControllerBase
  {
    private readonly DatabaseContext _context;

    public MovieController(DatabaseContext context)
    {
      _context = context;
    }

    // GET: api/Movie
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Movie>>> GetMovie()
    {
      var movies = await _context.Movie
        .Include(m => m.Reviews)
        .ToListAsync();

      return Ok(movies);
    }

    // GET: api/Movie/3
    [HttpGet("{id}")]
    public async Task<ActionResult<Review>> GetMovie(int id)
    {
      var movie = await _context.Movie
        .Include(m => m.Reviews)
        .FirstOrDefaultAsync(m => m.Id == id);

      if (movie == null)
      {
        return NotFound();
      }

      return Ok(movie);
    }

    // PUT: api/Movie/5
    [HttpPut("{id}"), Authorize]
    public async Task<IActionResult> PutMovie([Bind("Title,Year")]int id, Movie movie)
    {
      if (id != movie.Id)
      {
        return BadRequest();
      }

      _context.Entry(movie).State = EntityState.Modified;

      try
      {
        await _context.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException)
      {
        if (!MovieExists(id))
        {
          return NotFound();
        }
        else
        {
          throw;
        }
      }

      return NoContent();
    }

    // POST: api/Movie
    [HttpPost, Authorize]
    public async Task<ActionResult<Movie>> PostMovie([Bind("Title,Year")] Movie movie)
    {
      _context.Movie.Add(movie);
      await _context.SaveChangesAsync();

      return CreatedAtAction("GetMovie", new { id = movie.Id }, movie);
    }

    // DELETE: api/Movie/5
    [HttpDelete("{id}"), Authorize]
    public async Task<IActionResult> DeleteMovie(int id)
    {
      var movie = await _context.Movie.FindAsync(id);
      if (movie == null)
      {
        return NotFound();
      }

      _context.Movie.Remove(movie);
      await _context.SaveChangesAsync();

      return NoContent();
    }

    private bool MovieExists(int id)
    {
      return _context.Movie.Any(e => e.Id == id);
    }
  }
}
