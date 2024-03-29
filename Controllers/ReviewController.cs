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
using Microsoft.AspNetCore.Identity;

namespace imdb.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class ReviewController : ControllerBase
  {
    private readonly DatabaseContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ReviewController(DatabaseContext context, UserManager<ApplicationUser> userManager)
    {
      _context = context;
      _userManager = userManager;
    }

    // GET: api/Review
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Review>>> GetReviews()
    {
      var reviews = await _context.Review
        .Include(r => r.Movie)
        .ToListAsync();

      return Ok(reviews);
    }

    // GET: api/Review/3
    [HttpGet("{id}")]
    public async Task<ActionResult<Review>> GetReview(int id)
    {
      var review = await _context.Review
        .Include(r => r.Movie)
        .FirstOrDefaultAsync(r => r.Id == id);

      if (review == null)
      {
        return NotFound();
      }

      return Ok(review);
    }

    // PUT: api/Review/5
    [HttpPut("{id}"), Authorize]
    public async Task<IActionResult> PutReview(int id, Review review)
    {
      if (id != review.Id)
      {
        return BadRequest();
      }

      _context.Entry(review).State = EntityState.Modified;

      try
      {
        await _context.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException)
      {
        if (!ReviewExists(id))
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

    // POST: api/Review
    [HttpPost, Authorize]
    public async Task<ActionResult<Review>> PostReview(Review review)
    {
      review.UserId = _userManager.GetUserId(User);
      _context.Review.Add(review);
      await _context.SaveChangesAsync();

      return CreatedAtAction("GetReview", new { id = review.Id }, review);
    }

    // DELETE: api/Review/5
    [HttpDelete("{id}"), Authorize]
    public async Task<IActionResult> DeleteReview(int id)
    {
      var review = await _context.Review.FindAsync(id);
      if (review == null)
      {
        return NotFound();
      }

      _context.Review.Remove(review);
      await _context.SaveChangesAsync();

      return NoContent();
    }

    // GET: api/Review/MyReviews
    [HttpGet("MyReviews"), Authorize]
    public async Task<ActionResult<IEnumerable<Review>>> GetMyReviews()
    {
      var userId = _userManager.GetUserId(User);

      if (userId == null)
      {
        return NotFound();
      }

      var reviews = await _context.Review
        .Where(r => r.UserId == userId)
        .Include(r => r.Movie)
        .Include(r => r.User) 
        .ToListAsync();

      return Ok(reviews);
    }

    private bool ReviewExists(int id)
    {
      return _context.Review.Any(e => e.Id == id);
    }
  }
}
