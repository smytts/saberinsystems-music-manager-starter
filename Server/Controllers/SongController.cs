using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using music_manager_starter.Data;
using music_manager_starter.Data.Models;
using System;

namespace music_manager_starter.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SongsController : ControllerBase
    {
        private readonly DataDbContext _context;

        public SongsController(DataDbContext context)
        {
            _context = context;
        }

  
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Song>>> GetSongs()
        {
            return await _context.Songs.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Song>> PostSong(Song song)
        {
            if (song == null)
            {
                return BadRequest("Song cannot be null.");
            }


            _context.Songs.Add(song);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSong(Guid id)
        {
            // Find the song by ID
            var song = await _context.Songs.FindAsync(id);
            if (song == null)
            {
                return NotFound();
            }

            // Remove the song from the context
            _context.Songs.Remove(song);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Song>>> SearchSongs(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return await GetSongs(); // Return all songs if no query is provided
            }

            var lowerCaseQuery = query.ToLower();

            var results = await _context.Songs
                .Where(s => s.Title.ToLower().Contains(lowerCaseQuery) || s.Artist.ToLower().Contains(lowerCaseQuery) || s.Album.Contains(lowerCaseQuery))
                .ToListAsync();

            return Ok(results);
        }

    }
}
