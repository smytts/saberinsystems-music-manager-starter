using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using music_manager_starter.Data;
using music_manager_starter.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace music_manager_starter.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SongsController : ControllerBase
    {
        private readonly DataDbContext _context;
        private readonly ILogger<SongsController> _logger;

        //private readonly IWebHostEnvironment _environment;

        public SongsController(DataDbContext context, ILogger<SongsController> logger)
            //, IWebHostEnvironment environment)
        {
            _context = context;
            _logger = logger;
            // _environment = environment;
        }

  
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Song>>> GetSongs()
        {
            _logger.LogInformation("Getting all songs.");
            return await _context.Songs.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Song>> PostSong(Song song)
        {
            if (song == null)
            {
                _logger.LogWarning("PostSong: song cannot be null.");
                return BadRequest("Song cannot be null.");
            }


            _context.Songs.Add(song);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Added new song: {Title} by {Artist}.", song.Title, song.Artist); // Log information

            return Ok();
        }

        //[HttpPost("uploadAlbumArt/{songId}")]
        //public async Task<IActionResult> UploadAlbumArt(Guid songId, IFormFile file)
        //{
        //    if (file == null || file.Length == 0) {
        //        return BadRequest("No file uploaded.");
        //    }

        //    var song = await _context.Songs.FindAsync(songId);
        //    if (song == null)
        //    {
        //        return NotFound();
        //    }

        //    // Save file to directory
        //    var filename = $"{Guid.NewGuid()}_{file.FileName}";
        //    var filePath = Path.Combine(_environment.WebRootPath, "images", filename);

        //    using (var stream = new FileStream(filePath, FileMode.Create))
        //    {
        //        await file.CopyToAsync(stream);
        //    }

        //    // Update song's album art URL
        //    song.AlbumArtUrl = $"/images/{filename}";
        //    await _context.SaveChangesAsync();

        //    return Ok(song.AlbumArtUrl);
        //}

            [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSong(Guid id)
        {
            // Find the song by ID
            var song = await _context.Songs.FindAsync(id);
            if (song == null)
            {
                _logger.LogWarning("DeleteSong: No song found with ID {Id}.", id);
                return NotFound();
            }

            // Remove the song from the context
            _context.Songs.Remove(song);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Deleted song with ID {Id}.", id);

            return NoContent();
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Song>>> SearchSongs(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                _logger.LogInformation("SearchSongs: No query provided, returning all songs.");
                return await GetSongs(); // Return all songs if no query is provided
            }

            var lowerCaseQuery = query.ToLower();
            _logger.LogInformation("Searching songs with query: {Query}.", query);

            var results = await _context.Songs
                .Where(s => s.Title.ToLower().Contains(lowerCaseQuery) || s.Artist.ToLower().Contains(lowerCaseQuery) || s.Album.Contains(lowerCaseQuery))
                .ToListAsync();

            return Ok(results);
        }

    }
}
