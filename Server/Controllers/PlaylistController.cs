using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using music_manager_start.Data.Models;
using music_manager_starter.Data;
using music_manager_starter.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace music_manager_starter.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlaylistController : ControllerBase
    {
        private readonly DataDbContext _context;

        public PlaylistController(DataDbContext context)
        {
            _context = context;
        }

        // Get all playlists
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Playlist>>> GetPlaylists()
        {
            return await _context.Playlists
                .Include(p => p.Songs) // Include songs to load
                .ToListAsync();
        }

        // Get a specific playlist by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Playlist>> GetPlaylist(Guid id)
        {
            var playlist = await _context.Playlists
                .Include(p => p.Songs) // Include songs to load
                .FirstOrDefaultAsync(p => p.Id == id);

            if (playlist == null)
            {
                return NotFound();
            }
            return playlist;
        }

        // Create new playlist
        [HttpPost]
        public async Task<ActionResult<Playlist>> PostPlaylist(Playlist playlist)
        {
            if (playlist == null)
            {
                return BadRequest("Playlist cannot be null.");
            }

            _context.Playlists.Add(playlist);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPlaylist), new { id = playlist.Id }, playlist);
        }

        // Update an exisiting playlist
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlaylist(Guid id, Playlist updatedPlaylist)
        {
            if (id != updatedPlaylist.Id)
            {
                return BadRequest("Playlist ID mismatch.");
            }

            _context.Entry(updatedPlaylist).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlaylistExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // Delete a playlist
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlaylist(Guid id)
        {
            var playlist = await _context.Playlists.FindAsync(id);
            if (playlist == null)
            {
                return NotFound();
            }

            _context.Playlists.Remove(playlist);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PlaylistExists(Guid id)
        {
            return _context.Playlists.Any(playlist => playlist.Id == id);
        }
    }
}
