using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using music_manager_starter.Data;
using music_manager_starter.Data.Models;
using music_manager_starter.Shared.DTOs; // Import your DTOs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace music_manager_starter.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlaylistsController : ControllerBase
    {
        private readonly DataDbContext _context;

        public PlaylistsController(DataDbContext context)
        {
            _context = context;
        }

        // Get all playlists
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlaylistDTO>>> GetPlaylists() // Change return type to IEnumerable<PlaylistDTO>
        {
            var playlists = await _context.Playlists
                .Include(p => p.Songs) // Include songs to load
                .ToListAsync();

            // Map to DTOs
            var playlistDtos = playlists.Select(p => new PlaylistDTO
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Songs = p.Songs.Select(s => new SongDTO
                {
                    Id = s.Id,
                    Title = s.Title,
                    Artist = s.Artist
                }).ToList()
            }).ToList();

            return Ok(playlistDtos);
        }

        // Get a specific playlist by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<PlaylistDTO>> GetPlaylist(Guid id) // Change return type to PlaylistDTO
        {
            var playlist = await _context.Playlists
                .Include(p => p.Songs) // Include songs to load
                .FirstOrDefaultAsync(p => p.Id == id);

            if (playlist == null)
            {
                return NotFound();
            }

            // Map to DTO
            var playlistDto = new PlaylistDTO
            {
                Id = playlist.Id,
                Name = playlist.Name,
                Description = playlist.Description,
                Songs = playlist.Songs.Select(s => new SongDTO
                {
                    Id = s.Id,
                    Title = s.Title,
                    Artist = s.Artist
                }).ToList()
            };

            return Ok(playlistDto);
        }

        // Create new playlist
        [HttpPost]
        public async Task<ActionResult<PlaylistDTO>> PostPlaylist(PlaylistDTO playlistDto)
        {
            if (playlistDto == null)
            {
                return BadRequest("Playlist cannot be null.");
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                Console.WriteLine($"Model state errors: {string.Join(", ", errors.Select(e => e.ErrorMessage))}");
                return BadRequest("Model state is invalid");
            }

            if (playlistDto.SongIds == null || !playlistDto.SongIds.Any())
            {
                return BadRequest("At least one song must be included in the playlist.");
            }

            // Fetch existing songs from the database
            var existingSongs = await _context.Songs
                .Where(song => playlistDto.SongIds.Contains(song.Id))
                .ToListAsync();

            // Check if all songs exist
            if (existingSongs.Count != playlistDto.SongIds.Count)
            {
                return BadRequest("One or more songs do not exist in the library.");
            }

            // Create new playlist model
            var newPlaylist = new Playlist
            {
                Id = Guid.NewGuid(), // Generate a new ID
                Name = playlistDto.Name,
                Description = playlistDto.Description,
                Songs = existingSongs // Associate existing songs with the new playlist
            };

            Console.WriteLine($"Creating playlist: {newPlaylist.Name}, Songs: {string.Join(", ", newPlaylist.Songs.Select(s => s.Title))}");

            _context.Playlists.Add(newPlaylist);
            await _context.SaveChangesAsync();

            // Map the new playlist to DTO for the response
            playlistDto.Id = newPlaylist.Id; // Set the ID from the created playlist
            return CreatedAtAction(nameof(GetPlaylist), new { id = newPlaylist.Id }, playlistDto);
        }

        // Update an existing playlist
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlaylist(Guid id, PlaylistDTO updatedPlaylistDto) // Change parameter to PlaylistDTO
        {
            if (id != updatedPlaylistDto.Id)
            {
                return BadRequest("Playlist ID mismatch.");
            }

            // Fetch existing songs from the database
            var existingSongs = await _context.Songs
                .Where(song => updatedPlaylistDto.SongIds.Contains(song.Id))
                .ToListAsync();

            // Check if all songs exist
            if (existingSongs.Count != updatedPlaylistDto.SongIds.Count)
            {
                return BadRequest("One or more songs do not exist in the library.");
            }

            // Update the playlist
            var updatedPlaylist = new Playlist
            {
                Id = updatedPlaylistDto.Id,
                Name = updatedPlaylistDto.Name,
                Description = updatedPlaylistDto.Description,
                Songs = existingSongs // Associate existing songs with the updated playlist
            };

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
